using System;
using UnityEngine;

namespace VNTags
{
    [Serializable]
    public class VNBackgroundData : IVNData
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

        public string DataType { get; } = "Background";

        public static IVNData NoneDataStatic
        {
            get { return _None; }
        }
        private static           IVNData    _None = new VNBackgroundData();
    }
}