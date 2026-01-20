using UnityEngine;

namespace VNTags.ScriptAnimations
{
    public delegate void ScriptAnimationEvent();

    public abstract class ScriptAnimation : ScriptableObject
    {
        public ScriptAnimationEvent OnPause;
        public ScriptAnimationEvent OnResume;

        public ScriptAnimationEvent Onstart;
        public ScriptAnimationEvent OnStop;
        public abstract void Init(GameObject targetObject, MonoBehaviour runner);

        public abstract void Play(bool instant = false);

        public abstract void Pause();
        public abstract void Resume();
        public abstract void Skip();
    }
}