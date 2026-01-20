using System;
using UnityEngine;

namespace VNTags
{
    [Serializable]
    [CreateAssetMenu(fileName = "VNBackground", menuName = "VNTags/Data/VNBackground")]
    public class VNBackgroundData : ScriptableObject, IVNData
    {
        [SerializeField] private string     name;
        [SerializeField] private string[]   alias;
        [SerializeField] private GameObject prefab;

        public GameObject Prefab
        {
            get { return prefab; }
        }

        public string Name
        {
            get { return name; }
        }

        public string[] Alias
        {
            get { return alias; }
        }

        public         string           DataType { get; } = "Background";
        
        private static VNBackgroundData _none;
        public IVNData NoneData
        {
            get
            {
                return None;
            }
        }
        
        public static IVNData None
        {
            get
            {
                if (_none == null)
                {
                    _none = CreateInstance<VNBackgroundData>();
                    //_none.name = "None";
                }
                return _none;
            }
        }
    }
}