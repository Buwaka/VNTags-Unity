using System;
using UnityEngine;

namespace VNTags.Utility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PropertyRequireComponentAttribute : PropertyAttribute
    {
        public readonly Type Type;

        public PropertyRequireComponentAttribute(Type type)
        {
            Type = type;
        }
    }
}