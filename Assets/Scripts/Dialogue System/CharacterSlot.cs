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
        public Image ImageHolder;
        public Animator AnimatorHolder;
        private PlayableGraph _graph;
        private AnimationMixerPlayable _mixerPlayable;
        private AnimatorOverrideController _animatorOverrider;
        private AnimationClipPlayable _clipPlayable;

        private Vector3 _startPosition;

        private void Awake()
        {
            ImageHolder ??= GetComponent<Image>(); // if null set 
            AnimatorHolder ??= GetComponent<Animator>(); // if null set

            _graph = PlayableGraph.Create($"{gameObject.name}'s Graph"); // pulling out dark magic to make this work
            AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(_graph, "Anim output", AnimatorHolder);

            _mixerPlayable = AnimationMixerPlayable.Create(_graph, 1);
            Debug.Log(_mixerPlayable.GetInputCount());
            playableOutput.SetSourcePlayable(_mixerPlayable);

            AnimatorHolder.runtimeAnimatorController = null;
        }

        private void Start()
        {
            
            _startPosition = ImageHolder.transform.localPosition;
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
            ImageHolder.transform.rotation = Quaternion.identity;
            ImageHolder.transform.localPosition = _startPosition;


            if (_mixerPlayable.GetInputCount() > 0)
                _mixerPlayable.DisconnectInput(0);
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
            
            if (_mixerPlayable.GetInputCount() > 0)
                _mixerPlayable.DisconnectInput(0); // doing it again cause you never know


            _clipPlayable = AnimationClipPlayable.Create(_graph, animation);
            _mixerPlayable.ConnectInput(0, _clipPlayable, 0);
            _mixerPlayable.SetInputWeight(0, 1);

            _graph.Play();
        }

        /// <summary>
        /// since the playable system doesn't have garbage collection i have to do it
        /// </summary>
        private void OnDestroy()
        {
            if (_graph.IsValid())
                _graph.Destroy();

            // to be honest idk if i need to do this 2

            if (_mixerPlayable.IsValid())
                _mixerPlayable.Destroy();

            if (_clipPlayable.IsValid())
                _clipPlayable.Destroy();
        }
    }
}