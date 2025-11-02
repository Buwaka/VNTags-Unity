namespace VNTags.Tags
{
    public delegate bool SceneResetHandler(VNTagContext context);
    public class SceneResetTag : VNTag
    {
        public override    string          GetTagName()
        {
            return "SceneReset";
        }

        protected override VNTagParameters Parameters(VNTagParameters              currentParameters)
        {
            return new VNTagParameters();
        }
        protected override void   Execute(VNTagContext                    context, out    bool     isFinished)
        {
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onSceneReset?.Invoke(context));
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