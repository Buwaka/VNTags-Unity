using System;

namespace VNTags.Tags
{
    public delegate bool TransitionHandler(VNTagContext context, VNTransition expression);

    public class TransitionTag : VNTag
    {
        public static VNTransition DefaultTransition { get; set; } = null;
        public        VNTransition Transition        { get; private set; }

        public override string GetTagName()
        {
            return "transition";
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished =
                ExecuteHelper(
                              VNTagEventAnnouncer.onTransitionTag?.Invoke(context,
                                                                          Transition));
        }

        public override void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                Transition = VNTagsConfig.GetConfig().GetTransitionByNameOrAlias(parameters[0]);
            }
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            return Transition == null ? SerializeHelper(GetTagName()) : SerializeHelper(GetTagName(), Transition.Name);
        }
    }
}