using System;
using Eflatun.SceneReference;
using UnityEngine;

namespace VNTags
{
    [Serializable]
    public class VNScene : IVNData
    {


        // todo
        [SerializeField] private string         name;
        [SerializeField] private string[]       alias;
        [SerializeField] private SceneReference scene;
        [SerializeField] private Texture        thumbnail;

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