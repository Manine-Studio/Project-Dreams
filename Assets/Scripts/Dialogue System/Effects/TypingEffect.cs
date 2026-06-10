using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TypingEffect : MonoBehaviour
{

    [SerializeField] private float _SecondsDelayBetweenCharacters;
    private bool _CurrentSentenceFullyShown = false;
    [SerializeField] private TextMeshProUGUI _Sentence;

    private static TypingEffect _Instance;

    public static TypingEffect Instance => _Instance;
    public bool IsCurrentSentenceFinished { 
        get { 
            return _CurrentSentenceFullyShown || _Sentence == null || _Sentence.text == null || _Sentence.text == ""; 
        } 
    }

    private void Awake()
    {
        // 1. Logica Singleton
        if (_Instance == null)
        {
            _Instance = this;
        }
        else if (_Instance != this)
        {
            // Se esiste già un'altra istanza (es. caricata dalla nuova scena), 
            // distruggi immediatamente l'intero GameObject appena nato.
            Destroy(gameObject);
            return; // Esci per non eseguire il resto del codice
        }

    }
    public void StartTyping()
    {
        if(_Sentence != null && _Sentence.text != null && _Sentence.text != "")
        {
            _CurrentSentenceFullyShown = false;
            StartCoroutine(TypeSentenceCoroutine());
        }
    }

    public void TypeFully()
    {
        StopAllCoroutines();
        _Sentence.maxVisibleCharacters = _Sentence.textInfo.characterCount;
        _CurrentSentenceFullyShown = true;
    }
    


    private IEnumerator TypeSentenceCoroutine()
    {
        // Forza l'aggiornamento del testo per essere sicuri che i vertici siano calcolati
        _Sentence.ForceMeshUpdate();

        int totalVisibleCharacters = _Sentence.textInfo.characterCount;
        int counter = 0;

        // Imposta il testo inizialmente invisibile
        _Sentence.maxVisibleCharacters = 0;

        while (counter <= totalVisibleCharacters)
        {
            _Sentence.maxVisibleCharacters = counter;
            counter++;
            yield return new WaitForSeconds(_SecondsDelayBetweenCharacters);
        }

        _CurrentSentenceFullyShown = true;
    }
}
