using System;
using CharlieMadeAThing.NeatoTags.Core;
using Eflatun.SceneReference;
using UnityEngine;
using VNTags.SceneFlowControl;
using VNTags.SceneFlowControl.Utility;

namespace VNTags
{
    // NOTE: these events will also trigger on non-VNScenes, VNScene and VNController will be null in that instance
    public delegate void SceneGenericHandler(VNScene scene);
    public delegate void ScenePostLoadHandler(VNScene scene, BaseVNController controller);

    [CreateAssetMenu(fileName = "VNScene", menuName = "VNTags/Flow/Items/VNScene")]
    [Serializable]
    public class VNScene : FlowItem, IVNData
    {

        private static           VNScene        _none;
        [SerializeField] private string         name;
        [SerializeField] private string[]       alias;
        [SerializeField] private SceneReference scene;
        [SerializeField] private TextAsset      script;
        [SerializeField] private Texture        thumbnail;
        [SerializeField] private NeatoTag[]     tags;


        public SceneReference Scene
        {
            get { return scene; }
        }

        public TextAsset Script
        {
            get { return script; }
        }

        public Texture Thumbnail
        {
            get { return thumbnail; }
        }

        public static IVNData None
        {
            get
            {
                if (_none == null)
                {
                    _none = CreateInstance<VNScene>();
                    //_none.name = "None";
                }
                return _none;
            }
        }

        public string Name
        {
            get { return name; }
        }

        public string[] Alias
        {
            get { return alias; }
        }

        public string DataType { get; } = "Scene";
        public IVNData NoneData
        {
            get
            {
                return None;
            }
        }


        public override void Start()
        {
            // TODO create an instance of the VNController (perhaps set the class in the config) and pass it on to the scene somehow,
            // save the reference for safestate
            FlowControlUtility.LoadScene(this);
            onStart?.Invoke();
        }
        public override void Resume(IFlowSafeState state)
        {
            FlowControlUtility.ResumeScene(this, state);
        }
        public override void End(out bool isFinished)
        {
            VNTagEventAnnouncer.onSceneEnd?.Invoke(this);
            onEnd?.Invoke();
            isFinished = true;
            // throw new NotImplementedException();
        }
        public override IFlowSafeState Save()
        {
            throw new NotImplementedException();
        }
    }
}