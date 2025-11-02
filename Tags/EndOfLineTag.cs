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

        protected override VNTagParameters Parameters(VNTagParameters currentParameters)
        {
            return new VNTagParameters();
        }

        public override bool EditorVisible()
        {
            return false;
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onEndOfLineTag?.Invoke(context));
        }
    }
}