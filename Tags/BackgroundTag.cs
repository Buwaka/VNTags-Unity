using System;

namespace VNTags
{
    public class BackgroundTag : IVNTag
    {
        public VNBackground Background { get; }

        public void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            // todo
        }

        public string Serialize(VNTagSerializationContext context)
        {
            throw new NotImplementedException();
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