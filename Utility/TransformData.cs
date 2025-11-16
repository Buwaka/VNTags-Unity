using System;
using UnityEngine;

namespace VNTags.Utility
{
    [Serializable]
    public class TransformData
    {
        public Vector3 Position = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;
        public Vector3 Scale = Vector3.one;
    }
}