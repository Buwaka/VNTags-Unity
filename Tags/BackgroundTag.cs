namespace VNTags
{
    public class BackgroundTag : IVNTag
    {
        public VNBackground Background;
        
        public void Deserialize(string parameters, VNTagLineContext context)
        {
            // todo
        }

        public string Serialize()
        {
            throw new System.NotImplementedException();
        }

        public string GetTagID()
        {
            return "Background";
        }

        public void Execute(VNTagContext context, out bool isFinished)
        {
            // todo
            isFinished = true;
        }
    }
}