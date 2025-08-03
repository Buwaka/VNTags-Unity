using System.Collections.Generic;

namespace VNTags
{
    public class VNTagQueue : LinkedList<VNTag>
    {
        public LinkedList<VNTag> GetCollection()
        {
            return (LinkedList<VNTag>)this;
        }
        
        public void Tick(VNTagContext context)
        {
            if (Count <= 0 || First.Value == null)
            {
                return;
            }
            var tag = First.Value;
            
            tag.BaseExecute(context, out bool isFinished);

            if (isFinished)
            {
                RemoveFirst();
            }
        }
    }
}