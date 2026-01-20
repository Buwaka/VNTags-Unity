using UnityEngine;
using VNTags.SceneFlowControl.Utility;

namespace VNTags.SceneFlowControl
{
    public delegate void FlowEventHandler();
    public delegate void FlowNextEventHandler(FlowItem item);
    public delegate void FlowNewHandler(Flow flow);

    public abstract class Flow : ScriptableObject
    {
        public FlowEventHandler     onEnd;
        public FlowNextEventHandler onNext;
        public FlowEventHandler     onStart;


        public void StartFlow(VNScene sceneOnComplete = null)
        {
            if (sceneOnComplete != null)
            {
                onEnd += () => _onFlowEnd(sceneOnComplete);
            }

            VNTagEventAnnouncer.onNewFlow?.Invoke(this);
            Start();
        }

        private void _onFlowEnd(VNScene sceneOnComplete)
        {
            FlowControlUtility.LoadScene(sceneOnComplete);
        }

        protected abstract void Start();

        public abstract void Resume(IFlowSafeState safeState);

        public abstract bool End();

        public abstract FlowItem GetCurrent();

        public abstract FlowItem PeekNext();

        public abstract FlowItem MoveNext();

        public abstract IFlowSafeState GetSafeState();
    }
}