using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace VNTags
{
    [Serializable]
    public class VNBackgroundData : IVNData
    {
        [SerializeField] private string     name;
        [SerializeField] private string[]   alias;
        [SerializeField] private GameObject prefab;

        public string Name
        {
            get { return name; }
        }

        public string[] Alias
        {
            get { return alias; }
        }
    }
}