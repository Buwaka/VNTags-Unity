using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VNTags.Utility;

namespace VNTags.Tags
{
    public delegate bool TransitionHandler(VNTagContext context, VNTransitionData transition, IList<VNTag> tags);

    public class TransitionTag : VNTag
    {
        public static          VNTransitionData DefaultTransition { get; set; } = null;

        public              VNTransitionData Transition;
        [VNTagEditor] public string       MidTransitionTags = "";

        public override string GetTagName()
        {
            return "Transition";
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

            currentParameters.DefaultParameter(transitionParameter, Transition?.Name!);
            currentParameters.DefaultParameter(tagParameter,        MidTransitionTags);

            return currentParameters;
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            VNTransitionData trans = Transition ?? DefaultTransition;
            
            var tags = VNTagDeserializer.ParseLine(MidTransitionTags, ID.LineNumber);
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onTransitionTag?.Invoke(context, trans, tags));
        }

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            SetNone();
            if ((parameters != null) && (parameters.Length > 0))
            {
                Transition = VNTagsConfig.GetConfig().GetTransitionByNameOrAlias(parameters[0]);
            }
            
            if ((parameters != null) && (parameters.Length > 1))
            {
                MidTransitionTags = StringUtils.Unescape(parameters[1], EscapeCharacters);
            }

            return true;
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            if (Transition == null)
            {
                return "";
            }

            return Transition.IsNone() ? SerializeHelper(GetTagName()) : SerializeHelper(GetTagName(), Transition.Name, StringUtils.Escape(MidTransitionTags, EscapeCharacters));
        }

#if UNITY_EDITOR
        public ref VNTransitionData GetTransitionRef()
        {
            return ref Transition;
        }

        public void SetNone()
        {
            Transition = (VNTransitionData) VNTransitionData.None;
        }
#endif
    }
}