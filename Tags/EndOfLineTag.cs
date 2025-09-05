using System.Collections.Generic;

namespace VNTags.Tags
{
    public delegate bool EndOfLineHandler(VNTagContext context);

    public class EndOfLineTag : VNTag
    {
        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            return true;
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            return string.Empty;
        }

        public override string GetTagName()
        {
            return "EOL";
        }

        public override VNTagParameter[] GetParameters(IList<object> currentParameters)
        {
            return null!;
        }

        public override bool EditorVisibility()
        {
            return false;
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onEndOfLineTag?.Invoke(context));
        }
    }
}