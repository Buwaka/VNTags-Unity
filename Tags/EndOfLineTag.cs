namespace VNTags.Tags
{
    public delegate bool EndOfLineHandler(VNTagContext context);

    public class EndOfLineTag : VNTag
    {
        public override void Deserialize(VNTagDeserializationContext context, params string[] parameters) { }

        public override string Serialize(VNTagSerializationContext context)
        {
            return string.Empty;
        }

        public override string GetTagName()
        {
            return "EOL";
        }


        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished =
                ExecuteHelper(
                              VNTagEventAnnouncer.onEndOfLineTag?.Invoke(context));
        }
    }
}