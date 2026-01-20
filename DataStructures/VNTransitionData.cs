using System;
using UnityEngine;
using VNTags.Components;
using VNTags.Utility;

namespace VNTags
{
    [CreateAssetMenu(fileName = "NewTransitionData", menuName = "VNTags/Data/Transition Data")]
    public class VNTransitionData : ScriptableObject, IVNData
    {
        [SerializeField] private string name;

        [SerializeField] private string[] alias;

        [SerializeField] [PropertyRequireComponent(typeof(VNTransitionComponent))]
        public GameObject prefab;

        public string Name
        {
            get { return name; }
        }

        public string[] Alias
        {
            get { return alias; }
        }

        public string DataType { get; } = "Transition";

        public IVNData NoneData
        {
            get
            {
                return None;
            }
        }

        private static VNTransitionData _none;
        public static IVNData None
        {
            get
            {
                if (_none == null)
                {
                    _none = CreateInstance<VNTransitionData>();
                    //_none.name = "None";
                }
                return _none;
            }
        }

        public void Play() { }

        public override string ToString()
        {
            return name;
        }
    }
}