using UnityEngine;

namespace VNTags.Components
{
    public class VNSpriteBackgroundComponent : MonoBehaviour, IVNTransitionable
    {
        // todo basically from the VNController we call the SceneController to change the background,
        // the scene controller has the current background and receives the new one we want,
        // then first the VNSpriteBackgroundComponent of the current background is called to fade-out, 
        // the background tag is not finished, so each consecutive update will serve as a tick to the VNBackgroundComponent,
        // along with the option to skip the transition entirely,
        // do keep in mind that if the transition is skipped, both the fade in and the fadeout have to be skipped

        private bool _isTransitioning;


        public bool IsTransitioning()
        {
            return _isTransitioning;
        }

        public void Start()
        {
            _isTransitioning = true;
        }

        public void FadeIn(float progress)
        {
            foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, progress);
            }
        }

        public void FadeOut(float progress)
        {
            foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, progress);
            }
        }

        public void Finish()
        {
            _isTransitioning = false;
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}