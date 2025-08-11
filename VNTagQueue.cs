using System.Collections.Generic;

namespace VNTags
{
    public class VNTagQueue : LinkedList<VNTag>
    {
        public LinkedList<VNTag> GetCollection()
        {
            return this;
        }

        public void Tick(VNTagContext context)
        {
            if ((Count <= 0) || (First.Value == null))
            {
                return;
            }

            VNTag tag = First.Value;

            tag.BaseExecute(context, out bool isFinished);

            if (isFinished)
            {
                RemoveFirst();
            }
        }
    }
}