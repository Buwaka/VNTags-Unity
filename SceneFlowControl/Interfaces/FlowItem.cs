using UnityEngine;

namespace VNTags.SceneFlowControl
{
    public abstract class FlowItem : ScriptableObject
    {
        public FlowEventHandler onStart { get; set; }
        public FlowEventHandler onEnd   { get; set; }

        public abstract void Start();

        public abstract void Resume(IFlowSafeState state);

        public abstract void End(out bool isFinished);

        public abstract IFlowSafeState Save();
    }
}