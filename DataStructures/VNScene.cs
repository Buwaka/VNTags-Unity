using System;
using Eflatun.SceneReference;
using UnityEngine;

namespace VNTags
{
    [Serializable]
    public class VNScene : IVNData
    {
        [SerializeField] private string         name;
        [SerializeField] private string[]       alias;
        [SerializeField] private SceneReference scene;
        [SerializeField] private TextAsset      script;
        [SerializeField] private Texture        thumbnail;

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

        public string Name
        {
            get { return name; }
        }

        public string[] Alias
        {
            get { return alias; }
        }

        public string DataType { get; } = "Scene";
    }
}