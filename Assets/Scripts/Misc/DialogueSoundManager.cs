using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class DialogueSoundManager : MonoBehaviour
{
    private AudioSource _AudioSourceMusic1;
    private AudioSource _AudioSourceMusic2;
    private AudioSource _AudioSourceSFX;

    private AudioClip _ActualLooped;
    private float _ActualLoopedStartingPoint;
    private AudioClip _FutureLooped;
    private float _FutureLoopedStartingPoint;
    private double _CurrentAudioLength;

    private double _dScheduledStartTime= 0;
    private bool _isNextScheduled = false;
    private static DialogueSoundManager _Instance;

    public float SecondsForFading = 1.5f;

    // Proprietà pubblica per accedere al manager da altri script
    public static DialogueSoundManager Instance => _Instance;

    private void Awake()
    {
        // 1. Logica Singleton
        if (_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_Instance != this)
        {
            // Se esiste già un'altra istanza (es. caricata dalla nuova scena), 
            // distruggi immediatamente l'intero GameObject appena nato.
            Destroy(gameObject);
            return; // Esci per non eseguire il resto del codice
        }

        // 2. Inizializzazione (solo per l'istanza che sopravvive)
        SetupAudioSources();
    }

    private void SetupAudioSources()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        bool founded = false;
        foreach (var s in sources)
        {
            if (s.outputAudioMixerGroup.name == "Music")
            {
                if (founded)
                {

                    _AudioSourceMusic2 = s;
                } else
                {
                    founded = true;
                    _AudioSourceMusic1 = s;
                }
        }
            else if (s.outputAudioMixerGroup.name == "SFX")
                _AudioSourceSFX = s;
        }
    }

    public static void PlayOneShotSound(AudioClip clip)
    {
        _Instance._AudioSourceSFX.PlayOneShot(clip);
    }

    public static void PlayOnLoop(AudioClip clip, float startingPointLoop)
    {
        // Se non sta suonando nulla, partiamo normalmente
        if (_Instance._ActualLooped == null)
        {
            _Instance._ActualLooped = clip;
            _Instance._ActualLoopedStartingPoint = startingPointLoop;
            return;
        }

        // Se sta già suonando qualcosa, facciamo il Crossfade
        _Instance._FutureLooped = clip;
        _Instance._FutureLoopedStartingPoint = startingPointLoop;

        AudioSource sorgenteAttuale = _Instance._AudioSourceMusic1.isPlaying ?
                                      _Instance._AudioSourceMusic1 : _Instance._AudioSourceMusic2;
        AudioSource sorgenteNuova = (sorgenteAttuale == _Instance._AudioSourceMusic1) ?
                                     _Instance._AudioSourceMusic2 : _Instance._AudioSourceMusic1;

        _Instance.StartCoroutine(Crossfade(sorgenteAttuale, sorgenteNuova, _Instance.SecondsForFading));

    }

    public void Update()
    {
        if (_ActualLooped == null) return;

        // Se è in corso un Crossfade, non fare nulla
        if (_AudioSourceMusic1.isPlaying && _AudioSourceMusic2.isPlaying && _FutureLooped == null)
        {
            // Se entrambe suonano ma non c'è una "future" in coda, 
            // significa che siamo nel bel mezzo del Crossfade.
            return;
        }

        _CurrentAudioLength = _ActualLooped != null ? (double)_ActualLooped.samples / _ActualLooped.frequency : 0;
        bool music1IsPlaying = _AudioSourceMusic1.isPlaying;
        bool music2IsPlaying = _AudioSourceMusic2.isPlaying;
        //casistiche:
        // nessuno delle due music source sta suonando, ed esiste un actual loop -> lo riproduco e gestisco la clip
        if (!music1IsPlaying && !music2IsPlaying && _ActualLooped != null)
        {
            _AudioSourceMusic1.clip = _ActualLooped;
            _dScheduledStartTime = AudioSettings.dspTime;
            _AudioSourceMusic1.Play();
            _dScheduledStartTime += _CurrentAudioLength;
        }
        else if (music1IsPlaying)
        {
            // music 1 sta suonando
            SetFutureLoopPlaying(_AudioSourceMusic2, _AudioSourceMusic1);
        } else if (music2IsPlaying)
        {
            // music 2 sta suonando
            SetFutureLoopPlaying(_AudioSourceMusic1, _AudioSourceMusic2);
        }
        else
        {
            //caso di ActualLooped == null
            StopMusic();
        }


    }

    private void SetFutureLoopPlaying(AudioSource futurePlayingSource, AudioSource currentSourcePlaying)
    {
        // Se abbiamo già programmato il prossimo colpo, non fare nulla!
        if (_isNextScheduled) return;

        // Scheduliamo con un anticipo (es. 2 secondi prima della fine teorica)
        if (AudioSettings.dspTime >= _dScheduledStartTime - 2.0)
        {
            double durationOfClipToSchedule;
            futurePlayingSource.Stop();

            futurePlayingSource.clip = _ActualLooped;
            futurePlayingSource.volume = 1f;
            futurePlayingSource.timeSamples = Mathf.FloorToInt(_ActualLoopedStartingPoint * _ActualLooped.frequency);

            futurePlayingSource.SetScheduledStartTime(_dScheduledStartTime - _ActualLoopedStartingPoint);
            futurePlayingSource.PlayScheduled(_dScheduledStartTime);

            durationOfClipToSchedule = ((double)_ActualLooped.samples / _ActualLooped.frequency) - _ActualLoopedStartingPoint;
            

            _dScheduledStartTime += durationOfClipToSchedule;
            _isNextScheduled = true;
            StartCoroutine(ResetScheduleFlag());
        }
    }

    // Coroutine per permettere una nuova programmazione dopo che il cambio è avvenuto
    private IEnumerator ResetScheduleFlag()
    {
        // Aspetta finché non passiamo il punto di switch
        while (AudioSettings.dspTime < _dScheduledStartTime - 0.1)
        {
            yield return null;
        }
        _isNextScheduled = false;
    }

    public static void StopMusic()
    {
        if (_Instance != null)
        {
            _Instance._AudioSourceMusic1.Stop();
            _Instance._AudioSourceMusic2.Stop();
        }
    }

    private static IEnumerator Crossfade(AudioSource oldSource, AudioSource newSource, float duration)
    {
        // 1. Prepariamo la nuova sorgente
        newSource.clip = _Instance._FutureLooped;
        newSource.volume = 0;
        newSource.time = 0; // Parte dall'inizio (intro)
        newSource.Play();

        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            // Dissolvenza incrociata
            oldSource.volume = Mathf.Lerp(1f, 0f, progress);
            newSource.volume = Mathf.Lerp(0f, 1f, progress);

            yield return null;
        }

        // 2. Pulizia vecchia sorgente
        oldSource.Stop();
        oldSource.volume = 1f; // Reset volume per utilizzi futuri

        // 3. Allineamento variabili per il sistema Ping-Pong
        _Instance._ActualLooped = _Instance._FutureLooped;
        _Instance._ActualLoopedStartingPoint = _Instance._FutureLoopedStartingPoint;
        _Instance._FutureLooped = null;

        // Ricalcoliamo il dspStartTime basandoci su quando è PARTITA la nuova source
        // Sincronizziamo il prossimo loop alla fine di questa nuova clip
        double durationNuova = (double)_Instance._ActualLooped.samples / _Instance._ActualLooped.frequency;
        _Instance._dScheduledStartTime = AudioSettings.dspTime + (durationNuova - newSource.time);

        _Instance._isNextScheduled = false; // Permettiamo all'Update di schedulare il prossimo giro
    }
}
