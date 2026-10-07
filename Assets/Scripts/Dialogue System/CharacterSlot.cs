using Misc;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Image = UnityEngine.UI.Image;

namespace Dialogue_System
{
    /// <summary>
    /// this class is used to play animations to the characters in the dialogue
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CharacterSlot : MonoBehaviour
    {
        private Image _xImageHolder;
        private Animator _xAnimatorHolder;
        private PlayableGraph _xGraph;
        private AnimationMixerPlayable _xMixerPlayable;
        private AnimatorOverrideController _xAnimatorOverrider;
        private AnimationClipPlayable _xClipPlayable;
        private Vector3 _vStartPosition;
        
        public Image XImageHolder { get => _xImageHolder; }

        private void Awake()
        {
            _xImageHolder ??= GetComponent<Image>(); // if null set 
            _xAnimatorHolder ??= GetComponent<Animator>(); // if null set

            _xGraph = PlayableGraph.Create($"{gameObject.name}'s Graph"); // pulling out dark magic to make this work
            AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(_xGraph, "Anim output", _xAnimatorHolder);

            _xMixerPlayable = AnimationMixerPlayable.Create(_xGraph, 1);
            Debug.Log(_xMixerPlayable.GetInputCount());
            playableOutput.SetSourcePlayable(_xMixerPlayable);

            _xAnimatorHolder.runtimeAnimatorController = null;
        }

        private void Start()
        {
            _vStartPosition = _xImageHolder.transform.localPosition;
        }

        private void OnEnable()
        {
            GameManager.Instance.XDialogueEventBus.Register(DialogueEventList.RESET_CHARACTER, ResetCharacter);
        }

        private void OnDisable()
        {
            GameManager.Instance.XDialogueEventBus.Unregister(DialogueEventList.RESET_CHARACTER, ResetCharacter);
        }

        /// <summary>
        /// Called by the dialogue elaborator to reset any changes the previous animation did
        /// </summary>
        /// <param name="obj">nothin</param>
        private void ResetCharacter(object[] obj)
        {
            _xImageHolder.transform.rotation = Quaternion.identity;
            _xImageHolder.transform.localPosition = _vStartPosition;


            if (_xMixerPlayable.GetInputCount() > 0)
                _xMixerPlayable.DisconnectInput(0);
        }

        /// <summary>
        /// Called by the DialogueUiManager in the ChangeImage function <br></br>
        /// resets all inputs then cycles through every given animation and connects them with the playable system <br></br>
        /// then it plays them all at once 
        /// </summary>
        /// <param name="animation">the animations to play</param>
        public void PlayAnimation(AnimationClip animation)
        {
            if (animation == null)
                return;
            
            if (_xMixerPlayable.GetInputCount() > 0)
                _xMixerPlayable.DisconnectInput(0); // doing it again cause you never know


            _xClipPlayable = AnimationClipPlayable.Create(_xGraph, animation);
            _xMixerPlayable.ConnectInput(0, _xClipPlayable, 0);
            _xMixerPlayable.SetInputWeight(0, 1);

            _xGraph.Play();
        }

        /// <summary>
        /// since the playable system doesn't have garbage collection i have to do it
        /// </summary>
        private void OnDestroy()
        {
            if (_xGraph.IsValid())
                _xGraph.Destroy();

            // to be honest idk if i need to do this 2

            if (_xMixerPlayable.IsValid())
                _xMixerPlayable.Destroy();

            if (_xClipPlayable.IsValid())
                _xClipPlayable.Destroy();
        }
    }
}