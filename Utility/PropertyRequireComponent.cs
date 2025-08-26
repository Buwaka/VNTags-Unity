using System;
using UnityEngine;

namespace VNTags.Utility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PropertyRequireComponent : PropertyAttribute
    {
        public readonly Type Type;

        public PropertyRequireComponent(Type type)
        {
            Type = type;
        }
    }
}