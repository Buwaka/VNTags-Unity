using System;
using UnityEngine;

namespace VNTags
{
    /// <summary>
    ///     Name for the expression, to be used as a case-insensitive ID,
    ///     Expression gameobject will be attached as a child object to the VNCharacter object
    /// </summary>
    [Serializable]
    public class VNExpressionData : IVNData
    {
        private static           IVNData _None = new VNExpressionData();
        [SerializeField] private string  name;

        [Tooltip("Alternative names for writing convenience, case insensitive")] [SerializeField]
        private string[] alias;

        [SerializeField] private GameObject prefab;

        public GameObject Prefab
        {
            get { return prefab; }
        }

        public string Name
        {
            get { return name; }
        }

        public string[] Alias
        {
            get { return alias; }
        }

        public string DataType { get; } = "Expression";

        public static IVNData NoneDataStatic
        {
            get { return _None; }
        }
    }
}