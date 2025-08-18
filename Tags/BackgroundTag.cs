using System;

namespace VNTags.Tags
{
    public class BackgroundTag : VNTag
    {
        public VNBackgroundData Background { get; }

        public override void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            // todo
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetTagName()
        {
            return "Background";
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            // todo
            isFinished = true;
        }
    }
}