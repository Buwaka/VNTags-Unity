using System;
using System.Collections.Generic;
using System.Linq;

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

        public override VNTagParameter[] GetParameters(IList<object> currentParameters)
        {
            return new[]
            {
                new VNTagParameter("Transition Name",
                                   TypeCode.String,
                                   "Name of the transition to be played",
                                   null,
                                   true,
                                   null,
                                   VNTagsConfig.GetConfig().AllTransitions.Select(t => t.name).ToArray())
            };
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onTransitionTag?.Invoke(context, Transition));
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