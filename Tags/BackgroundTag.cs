using System;
using System.Collections.Generic;

namespace VNTags.Tags
{
    public class BackgroundTag : VNTag
    {
        public VNBackgroundData Background { get; }

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            // todo
            return true;
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetTagName()
        {
            return "Background";
        }

        public override VNTagParameter[] GetParameters(IList<object> currentParameters)
        {
            return null;
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            // todo
            isFinished = true;
        }
    }
}