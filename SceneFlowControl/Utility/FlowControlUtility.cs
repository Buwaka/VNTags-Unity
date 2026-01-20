using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VNTags.SceneFlowControl.Utility
{
    public static class FlowControlUtility
    {
        private static void _LoadScene(SceneReference scene)
        {
            SceneManager.LoadScene(scene.Name);
        }

        public static void LoadScene(SceneReference scene)
        {
            // VNTagEventAnnouncer.onPreSceneStart?.Invoke(null, scene);
            _LoadScene(scene);
            // VNTagEventAnnouncer.onPostSceneStart?.Invoke(null, scene, null);
        }

        public static void LoadScene(VNScene scene)
        {
            VNTagEventAnnouncer.onPreSceneStart?.Invoke(scene);
            _LoadScene(scene.Scene);

            var controller = GameObject.FindFirstObjectByType<BaseVNController>(FindObjectsInactive.Exclude);
            if (controller != null)
            {
                controller.Init(scene.Script);
            }

            VNTagEventAnnouncer.onPostSceneStart?.Invoke(scene, controller);
        }
        public static void ResumeScene(VNScene scene, IFlowSafeState state)
        {
            VNTagEventAnnouncer.onPreSceneStart?.Invoke(scene);
            _LoadScene(scene.Scene);

            var controller = GameObject.FindFirstObjectByType<BaseVNController>(FindObjectsInactive.Include);
            if (controller != null)
            {
                controller.Init(scene.Script, state);
            }

            VNTagEventAnnouncer.onPostSceneStart?.Invoke(scene, controller);
        }
    }
}