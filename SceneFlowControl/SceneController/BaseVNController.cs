using UnityEngine;

namespace VNTags.SceneFlowControl
{
    public delegate void VNControllerCreated(BaseVNController controller);
    
    public abstract class BaseVNController : MonoBehaviour
    {
        /// <summary>
        ///     Set by FlowControlUtility.ResumeScene just before it loads the scene that should resume
        ///     with this state, and consumed by that scene's own controller from its own Start(). We
        ///     can't find-and-Init the controller synchronously right after calling SceneManager.LoadScene:
        ///     the scene swap isn't synchronous, so a find right there can still see the scene being left.
        ///     Routing the handoff through Start() uses the same timing that already reliably works for a
        ///     fresh (non-resume) load.
        /// </summary>
        public static VNScene        PendingResumeScene;
        public static IFlowSafeState PendingResumeState;

        /// <param name="autoPaceLines">
        ///     When true, dialogue lines pace themselves (see VNTagDeserializer.Parse) instead of waiting
        ///     on player input — for flow-graph-driven content rather than the main, player-paced scene.
        /// </param>
        public abstract void Init(TextAsset script, IFlowSafeState state = null, bool autoPaceLines = false);

        public abstract IFlowSafeState Save();

        public abstract void Exit();
    }
}