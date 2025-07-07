namespace VNTags
{
    public class BackgroundTag : IVNTag
    {
        private VNBackground _background;

        public VNBackground Background => _background;

        public void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            // todo
        }

        public string Serialize(VNTagSerializationContext context)
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