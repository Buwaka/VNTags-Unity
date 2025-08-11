using System;
using UnityEngine;

namespace VNTags
{
    /// <summary>
    ///     Name for the expression, to be used as a case-insensitive ID,
    ///     Expression gameobject will be attached as a child object to the VNCharacter object
    /// </summary>
    [Serializable]
    public class VNExpressionData
    {
        public string Name;

        [Tooltip("Alternative names for writing convenience, case insensitive")]
        public string[] Alias;

        public GameObject Object;
    }
}