using UnityEngine;

namespace VNTags.SceneFlowControl
{
    public delegate void VNControllerCreated(BaseVNController controller);
    
    public abstract class BaseVNController : MonoBehaviour
    {
        public abstract void Init(TextAsset script, IFlowSafeState state = null);

        public abstract IFlowSafeState Save();

        public abstract void Exit();
    }
}