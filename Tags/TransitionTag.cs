using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VNTags.Tags
{
    public delegate bool TransitionHandler(VNTagContext context, VNTransition transition, IList<VNTag> tags);

    public class TransitionTag : VNTag
    {
        public static          VNTransition DefaultTransition { get; set; } = null;

        public              VNTransition Transition;
        [VNTagEditor] public string       MidTransitionTags = "";

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

            var tagParameter = new VNTagParameter(2, "Mid Transition Tags", TypeCode.Object, "Tags to be processed during the transition", true);

            currentParameters.UpdateParameter(transitionParameter, Transition);
            currentParameters.UpdateParameter(tagParameter,        MidTransitionTags);

            return currentParameters;
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            VNTransition trans = Transition ?? DefaultTransition;
            
            var tags = VNTagDeserializer.ParseLine(MidTransitionTags, ID.LineNumber);
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onTransitionTag?.Invoke(context, trans, tags));
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
            if (Transition.IsNone())
            {
                return "";
            }

            return Transition == null ? SerializeHelper(GetTagName()) : SerializeHelper(GetTagName(), Transition.Name);
        }

#if UNITY_EDITOR
        public ref VNTransition GetTransitionRef()
        {
            return ref Transition;
        }

        public void SetNone()
        {
            Transition = (VNTransition) VNTransition.NoneDataStatic;
        }
#endif
    }
}