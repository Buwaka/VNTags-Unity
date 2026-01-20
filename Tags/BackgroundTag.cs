using System;
using UnityEngine;

namespace VNTags.Tags
{
    public delegate bool BackgroundHandler(VNTagContext context, VNBackgroundData background);

    public class BackgroundTag : VNTag
    {
        private VNBackgroundData _background;

        public VNBackgroundData Background
        {
            get { return _background; }
            private set { _background = value; }
        }

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
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
            if (Background.IsNone())
            {
                return "";
            }
            return Background != null && !Background.IsNone() ? SerializeHelper(GetTagName(), Background.Name) : "";
        }

        public override string GetTagName()
        {
            return "Background";
        }

        protected override VNTagParameters Parameters(VNTagParameters currentParameters)
        {
            var backgroundParameter = new VNTagParameter(1,
                                                         "Background",
                                                         TypeCode.String,
                                                         "Background to set the scene to",
                                                         false,
                                                         null,
                                                         VNTagsConfig.GetConfig().GetBackgroundNames());

            currentParameters.DefaultParameter(backgroundParameter, Background?.Name!);
            return currentParameters;
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onBackgroundTag?.Invoke(context, Background));
        }

#if UNITY_EDITOR
        public ref VNBackgroundData GetBackgroundRef()
        {
            return ref _background;
        }

        public void SetNone()
        {
            _background = (VNBackgroundData)VNBackgroundData.None;
        }
#endif
    }
}