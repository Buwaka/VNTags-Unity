using System;
using System.Linq;

namespace VNTags.Tags
{
    public delegate bool TransitionHandler(VNTagContext context, VNTransition transition);

    public class TransitionTag : VNTag
    {
        public static VNTransition DefaultTransition { get; set; } = null;
        public        VNTransition Transition        { get; private set; }

        public override string GetTagName()
        {
            return "transition";
        }

        protected override VNTagParameters Parameters(VNTagParameters currentParameters)
        {
            var transitionParameter = new VNTagParameter(1,
                                                         "Transition Name",
                                                         TypeCode.String,
                                                         "Name of the transition to be played",
                                                         true,
                                                         null,
                                                         VNTagsConfig.GetConfig().AllTransitions.Select(t => t.Name).ToArray());

            var tagParameter = new VNTagParameter(2, "Transition Name", TypeCode.Object, "Name of the transition to be played");

            currentParameters.UpdateParameter(transitionParameter, Transition);
            currentParameters.UpdateParameter(tagParameter,        null);

            return currentParameters;
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            VNTransition trans = Transition ?? DefaultTransition;
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onTransitionTag?.Invoke(context, trans));
        }

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if ((parameters != null) && (parameters.Length > 0))
            {
                Transition = VNTagsConfig.GetConfig().GetTransitionByNameOrAlias(parameters[0]);
            }

            return true;
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            return Transition == null ? SerializeHelper(GetTagName()) : SerializeHelper(GetTagName(), Transition.Name);
        }
    }
}