using System;
using UnityEngine;
using VNTags.SceneFlowControl;

namespace VNTags
{
    [CreateAssetMenu(fileName = "VNChapter", menuName = "VNTags/Flow/Items/Chapter")]
    [Serializable]
    public class VNChapter : FlowItem, IVNData
    {

        private static               VNChapter _none;
        [SerializeField]     private string    name;
        [SerializeField]     private string[]  alias;
        [SerializeReference] private VNScene[] scenes;

        public static IVNData None
        {
            get
            {
                if (_none == null)
                {
                    _none = CreateInstance<VNChapter>();
                    //_none.name = "None";
                }
                return _none;
            }
        }

        public string   Name     { get { return name; } }
        public string[] Alias    { get { return alias; } }
        public string   DataType { get; } = "Chapter";
        public IVNData NoneData
        {
            get
            {
                return None;
            }
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }
        public override void Resume(IFlowSafeState state)
        {
            throw new NotImplementedException();
        }
        public override void End(out bool isFinished)
        {
            throw new NotImplementedException();
        }
        public override IFlowSafeState Save()
        {
            throw new NotImplementedException();
        }
    }
}