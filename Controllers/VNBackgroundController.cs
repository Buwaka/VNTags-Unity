using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VNTags.Controllers
{
    public class VNBackgroundController : MonoBehaviour
    {
        public           float                                             defaultTransitionTime = 3.0f;
        private readonly Dictionary<VNBackgroundData, IVNTransitionable[]> _backgrounds          = new();

        private GameObject       _backgroundContainer;
        private VNBackgroundData _currentBackground;
        private bool             _skipTransition;
        private VNBackgroundData _targetBackground;
        private float            _timer;
        private Coroutine        _transitionCoroutine;

        private float _transitionTime;


        private void Start()
        {
            InitializeBackgrounds();
        }

        private void InitializeBackgrounds()
        {
            _backgroundContainer = new GameObject("BackgroundContainer");

            foreach (VNBackgroundData background in VNTagsConfig.GetConfig().AllBackgrounds)
            {
                GameObject obj = Instantiate(background.Prefab, _backgroundContainer.transform);

                var transitionables = new List<IVNTransitionable>();
                foreach (MonoBehaviour component in obj.GetComponents<MonoBehaviour>())
                {
                    if (component is IVNTransitionable transitionable)
                    {
                        transitionables.Add(transitionable);
                        transitionable.Disable();
                    }
                }

                _backgrounds.Add(background, transitionables.ToArray());
                if (transitionables.Count <= 0)
                {
                    Debug.LogError($"VNSceneController: InitializeBackgrounds: background (${background.Name}) does not contain a component that implements IVNTransitionable");
                }
            }
        }

        /// <summary>
        ///     Start transitioning to a new background.
        ///     The first call is to start the transition, consecutive calls are to check whether the transition has been completed
        ///     or to change the transition time.
        ///     Default transition time will be used
        /// </summary>
        /// <param name="background">New background to transition to</param>
        /// <param name="instant">whether this transition should happen instantaneously, will always return true</param>
        /// <returns>whether the transition has been completed</returns>
        public bool ChangeBackground(VNBackgroundData background, bool instant = false)
        {
            return ChangeBackground(background, defaultTransitionTime, instant);
        }


        /// <summary>
        ///     Start transitioning to a new background.
        ///     The first call is to start the transition, consecutive calls are to check whether the transition has been completed
        ///     or to change the transition time
        /// </summary>
        /// <param name="background">New background to transition to</param>
        /// <param name="time">how much time in seconds to spend on the transition, can't be 0, use instant</param>
        /// <param name="instant">whether this transition should happen instantaneously, will always return true</param>
        /// <returns>whether the transition has been completed</returns>
        public bool ChangeBackground(VNBackgroundData background, float time, bool instant = false)
        {
            if (background == null)
            {
                Debug.LogError("VNSceneController: ChangeBackground: background is null, exiting");
                return true;
            }

            if (background == _currentBackground)
            {
                return true;
            }

            _transitionTime = time;
            _skipTransition = instant;
            if (_targetBackground != background && _targetBackground != null)
            {
                Debug.LogError("VNSceneController: ChangeBackground: different background is still transitioning, "
                             + "transition will be finished but call this function again for the new transition, exiting");
                return false;
            }

            if (_transitionCoroutine != null)
            {
                return false;
            }

            _targetBackground    = background;
            _transitionCoroutine = StartCoroutine(TransitionBackground());
            return _skipTransition;
        }

        private IEnumerator TransitionBackground()
        {
            _timer = 0.0f;

            //fade-out current background
            if (_currentBackground != null)
            {
                foreach (IVNTransitionable transitionable in _backgrounds[_currentBackground])
                {
                    transitionable.Start();
                }

                while ((_timer < _transitionTime / 2) && !_skipTransition)
                {
                    foreach (IVNTransitionable transitionable in _backgrounds[_currentBackground])
                    {
                        float progress = Mathf.Clamp(_timer / (_transitionTime / 2), 0, 1.0f);
                        transitionable.FadeOut(progress);
                    }

                    yield return null;
                    _timer += Time.deltaTime;
                }

                foreach (IVNTransitionable transitionable in _backgrounds[_currentBackground])
                {
                    transitionable.FadeOut(1.0f);
                    transitionable.Finish();
                    transitionable.Disable();
                }
            }

            //fade-in new background
            if (_targetBackground != null)
            {
                foreach (IVNTransitionable transitionable in _backgrounds[_targetBackground])
                {
                    transitionable.Enable();
                    transitionable.Start();
                }

                while ((_timer < _transitionTime) && !_skipTransition)
                {
                    foreach (IVNTransitionable transitionable in _backgrounds[_targetBackground])
                    {
                        float total    = (_transitionTime / 2);
                        float current  = _timer;
                        float progress = Mathf.Clamp( current / total, 0, 1.0f);
                        transitionable.FadeIn(progress);
                    }

                    yield return null;
                    _timer += Time.deltaTime;
                }

                foreach (IVNTransitionable transitionable in _backgrounds[_targetBackground])
                {
                    transitionable.FadeIn(1.0f);
                    transitionable.Finish();
                }
            }

            _currentBackground   = _targetBackground;
            _targetBackground    = null;
            _transitionCoroutine = null;
        }
    }
}