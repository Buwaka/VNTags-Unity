namespace VNTags.Tags
{
    public delegate bool ToggleVNUIHandler(VNTagContext context);
    public class ToggleVNUI : VNTag
    {
        public override    string          GetTagName()
        {
            return "ToggleUI";
        }
        protected override VNTagParameters Parameters(VNTagParameters              currentParameters)
        {
            return new VNTagParameters();
        }
        protected override void   Execute(VNTagContext                    context, out    bool     isFinished)
        {
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onToggleUI?.Invoke(context));
        }
        public override bool   Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            return true;
        }
        public override string Serialize(VNTagSerializationContext     context)
        {
            return SerializeHelper(GetTagName());
        }
    }
}