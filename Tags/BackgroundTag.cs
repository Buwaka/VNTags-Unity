using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace VNTags.Tags
{
    public delegate bool BackgroundHandler(VNTagContext context, VNBackgroundData background);
    public class BackgroundTag : VNTag
    {
        public VNBackgroundData Background { get; private set; }

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if ((parameters == null) || (parameters.Length <= 0))
            {
                Debug.LogError("BackgroundTag: Deserialize: No parameters provided '" + context + "'");
                return false;
            }

            Background = VNTagsConfig.GetConfig().GetBackgroundByNameOrAlias(parameters[0]);

            if (Background == null)
            {
                Debug.LogError("BackgroundTag: Deserialize: Failed to find Background with name '" + parameters[0] + "', " + context);
            }

            return true;
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            return SerializeHelper(GetTagName(), Background.Name);
        }

        public override string GetTagName()
        {
            return "Background";
        }

        public override VNTagParameter[] GetParameters(IList<object> currentParameters)
        {
            return new[]
            {
                new VNTagParameter("Background",
                                   TypeCode.String,
                                   "Background to set the scene to",
                                   null,
                                   false,
                                   null,
                                   VNTagsConfig.GetConfig().GetBackgroundNames())
            };
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onBackgroundTag?.Invoke(context, Background));
        }
    }
}