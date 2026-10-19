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

            // Handed off to whichever controller's own Start() picks this scene back up, rather than
            // found-and-Init'd here: SceneManager.LoadScene doesn't swap the active scene synchronously,
            // so finding a controller immediately after calling it can still see the scene we're leaving.
            BaseVNController.PendingResumeScene = scene;
            BaseVNController.PendingResumeState = state;

            _LoadScene(scene.Scene);

            VNTagEventAnnouncer.onPostSceneStart?.Invoke(scene, null);
        }
    }
}