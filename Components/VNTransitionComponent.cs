using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using VNTags.Utility;

namespace VNTags.Components
{
    public enum VNTransitionEvent
    {
        Start,
        FullScreen,
        Reveal,
        Finished,
        Custom
    }

    public delegate void TransitionEventHandler(VNTransitionComponent component, VNTransitionEvent transitionEvent, [CanBeNull] string customEvent);

    [RequireComponent(typeof(Animator))]
    public class VNTransitionComponent : MonoBehaviour
    {
        [InfoField("Be sure to add Animation Events that call each respective function in this class,\n"
                 + "Click on the gameObject, go to the animation tab, \nright click on the horizontal bar under the timeline,\n"
                 + "\"add event\", name it as you please \n(I recommend just naming it the same as the function it will call),\n"
                 + "then in the inspector for that event, select the function to assign to that event, \nVNTags will then announce this event through the VNTagAnnouncer.\n\n"
                 + "Events:\n"
                 + "- StartAnimation: When the animation starts playing\n"
                 + "- FullScreen: when the transition has fully covered the screen, \nthis is the prime moment to change the background or do any kind abrupt scene changes\n"
                 + "- Reveal: when the animation is starting to reveal the scene again, \nthis can be used to pause the animation in case we need more loading time\n"
                 + "- Finished: Animation has fully finished playing, \nnot sure what you'd use this for, but potentially useful\n"
                 + "- Custom: this allows you to pass a string value along with the event, in case the premade events aren't sufficient")]
        [DisableField]
        [SerializeField]
        private string EVENTS = "Read Above";

        private readonly VNTagContext _context = new();

        private Animator   _animator;
        private VNTagQueue _midTransitionTags;


        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void StartAnimation()
        {
            VNTagEventAnnouncer.onTransitionEvent?.Invoke(this, VNTransitionEvent.Start, null);
        }

        public void FullScreen()
        {
            VNTagEventAnnouncer.onTransitionEvent?.Invoke(this, VNTransitionEvent.FullScreen, null);
        }

        public void Reveal()
        {
            if (_midTransitionTags.ExecuteAll(_context))
            {
                VNTagEventAnnouncer.onTransitionEvent?.Invoke(this, VNTransitionEvent.Reveal, null);
            }
            else
            {
                StartCoroutine(_midTransitionTags.ExecuteAsync(_context,
                                                               () =>
                                                               {
                                                                   VNTagEventAnnouncer.onTransitionEvent?.Invoke(this, VNTransitionEvent.Reveal, null);
                                                               }));
            }

        }

        public void Finished()
        {
            VNTagEventAnnouncer.onTransitionEvent?.Invoke(this, VNTransitionEvent.Finished, null);
            Destroy(gameObject);
        }

        public void CustomEvent(string customValue)
        {
            VNTagEventAnnouncer.onTransitionEvent?.Invoke(this, VNTransitionEvent.Custom, customValue);
        }

        public void SetMidTransitionTags(IList<VNTag> tags)
        {
            _midTransitionTags = new VNTagQueue(tags);
        }
    }
}