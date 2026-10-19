using UnityEngine;

namespace VNTags.SceneFlowControl
{
    /// <summary>
    ///     A flow wrapping exactly one FlowItem. Unlike LinearFlow there is no "next" item to advance to:
    ///     ending the item always ends the whole flow. Used for one-off cutaways (e.g. SceneTag) where a
    ///     caller wants a FlowItem to run to completion and be notified via onEnd, without authoring a
    ///     full LinearFlow asset for it.
    /// </summary>
    public class SingleItemFlow : Flow
    {
        public FlowItem item;

        protected override void Start()
        {
            if (item == null)
            {
                Debug.LogError("SingleItemFlow: Start: no item assigned, aborting");
                return;
            }

            item.Start();
            onStart?.Invoke();
        }

        public override void Resume(IFlowSafeState safeState)
        {
            if (item == null)
            {
                Debug.LogError("SingleItemFlow: Resume: no item assigned, aborting");
                return;
            }

            item.Resume(safeState);
            onStart?.Invoke();
        }

        public override bool End()
        {
            bool finished = true;
            item?.End(out finished);

            if (finished)
            {
                onEnd?.Invoke();
            }

            return finished;
        }

        public override FlowItem GetCurrent()
        {
            return item;
        }

        public override FlowItem PeekNext()
        {
            return null;
        }

        public override FlowItem MoveNext()
        {
            return null;
        }

        public override IFlowSafeState GetSafeState()
        {
            return item?.Save();
        }
    }
}
