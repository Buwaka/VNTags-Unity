using System.Collections;
using UnityEngine;

namespace VNTags.ScriptAnimations
{
    [CreateAssetMenu(menuName = "ScriptAnimation/HorizontalFadeOut", fileName = "HorizontalFadeOut")]
    public class HorizontalFadeOut : ScriptAnimation
    {
        public float relativeDistance = 3.0f;
        public float animetionTime    = 1.0f;
        
        private GameObject _target;
        private MonoBehaviour _runner;
        private Vector3  _targetlocation;
        private bool _paused = false;
        private bool _skip = false;
        
        
        
        public override void Init(GameObject targetObject, MonoBehaviour runner)
        {
            _target = targetObject;
            _runner = runner;
            if (targetObject == null)
            {
                Debug.LogError("SimpleMoveIn: Init: targetObject is null");
            }
        }
        
        public override void Play(bool instant = false)
        {
            if (!instant)
            {
                _targetlocation = (Vector3.left * relativeDistance) + _target.transform.position;
                _runner.StartCoroutine(Animationcoroutine());
            }
        }
        public override void Pause()
        {
            _paused = true;
            OnPause?.Invoke();
        }
        public override void Resume()
        {
            _paused = false;
            OnResume?.Invoke();
        }
        public override void Skip()
        {
            _skip = true;
            OnStop?.Invoke();
        }

        IEnumerator Animationcoroutine()
        {
            Onstart?.Invoke();
            var sprites = _target.GetComponentsInChildren<SpriteRenderer>(true);
            float time = 0.0f;
            Vector3 startPos = _target.transform.position;
            while (time < animetionTime && !_skip)
            {
                if (_paused)
                {
                    yield return null;
                }
                
                float progress = time / animetionTime;

                foreach (var sprite in sprites)
                {
                    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1 - progress);
                }
                
                _target.transform.position = Vector3.Lerp(startPos, _targetlocation, progress);
                time += Time.deltaTime;
                yield return null;
            }
            _target.transform.position =  _targetlocation;
            OnStop?.Invoke();
        }
    }
}