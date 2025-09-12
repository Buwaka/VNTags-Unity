using System;
using UnityEngine;
using VNTags.Components;
using VNTags.Utility;

namespace VNTags
{
    [Serializable]
    public class VNTransition : IVNData
    {
        public string name;

        public string[] alias;

        [PropertyRequireComponent(typeof(VNTransitionComponent))]
        public GameObject prefab;


        public string Name
        {
            get { return name; }
        }

        public string[] Alias
        {
            get { return alias; }
        }


        public void Play() { }
    }
}