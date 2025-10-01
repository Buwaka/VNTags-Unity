using System;
using UnityEngine;

namespace VNTags.Utility
{
    [Serializable]
    public struct SortingLayerPicker
    {
        public int id;

        public string Name
        {
            get { return SortingLayer.IDToName(id); }
        }

        public static implicit operator int(SortingLayerPicker layerPicker)
        {
            return layerPicker.id;
        }
    }
}