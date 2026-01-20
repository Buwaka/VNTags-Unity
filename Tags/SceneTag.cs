using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VNTags.Tags
{
    public delegate bool SceneHandler(VNTagContext context, VNScene scene);

    public class SceneTag : VNTag
    {
        private VNScene _scene;
        
        public override string GetTagName()
        {
            return "Scene";
        }

        protected override VNTagParameters Parameters(VNTagParameters currentParameters)
        {
            var sceneNameParameter = new VNTagParameter(1,
                                                         "SceneName",
                                                         TypeCode.String,
                                                         "name of the scene to change to",
                                                         false,
                                                         null,
                                                         VNTagsConfig.GetConfig().GetSceneNames());

            currentParameters.DefaultParameter(sceneNameParameter, _scene?.Name!);
            return currentParameters;
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onSceneTag?.Invoke(context, _scene));
        }

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if ((parameters == null) || (parameters.Length <= 0))
            {
                Debug.LogError("SceneTag: Deserialize: No parameters provided '" + context + "'");
                return false;
            }

            _scene = VNTagsConfig.GetConfig().GetSceneByName(parameters[0]);

            if (_scene == null)
            {
                Debug.LogError("SceneTag: Deserialize: Failed to find VNScene with name '" + parameters[0] + "', " + context);
            }

            return true;
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            if (_scene.IsNone())
            {
                return "";
            }
            return (_scene != null) && !_scene.IsNone() ? SerializeHelper(GetTagName(), _scene.Name) : "";
        }
    }
}