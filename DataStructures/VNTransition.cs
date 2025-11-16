using System;
using UnityEngine;
using VNTags.Components;
using VNTags.Utility;

namespace VNTags
{
    [Serializable]
    public class VNTransition : IVNData
    {
        [SerializeField] private string  name;

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

        public static IVNData NoneDataStatic
        {
            get { return _none; }
        }
        private static           IVNData _none = new VNTransition();

        public void Play() { }

        public override string ToString()
        {
            return name;
        }
    }
}