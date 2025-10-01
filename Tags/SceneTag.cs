using System;

namespace VNTags.Tags
{
    public delegate bool SceneHandler(VNTagContext context, VNScene scene);

    public class SceneTag : VNTag
    {
        public override string GetTagName()
        {
            return "Scene";
        }

        protected override VNTagParameters Parameters(VNTagParameters currentParameters)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            throw new NotImplementedException();
        }

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            throw new NotImplementedException();
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            throw new NotImplementedException();
        }
    }
}