using System;
using UnityEngine;

namespace VNTags
{
    /// <summary>
    ///     Name for the expression, to be used as a case-insensitive ID,
    ///     Expression gameobject will be attached as a child object to the VNCharacter object
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "VNExpression", menuName = "VNTags/Data/VNExpression")]
    public class VNExpressionData : ScriptableObject, IVNData
    {

        private static           VNExpressionData _none;
        [SerializeField] private string           name;

        [Tooltip("Alternative names for writing convenience, case insensitive")] [SerializeField]
        private string[] alias;

        [SerializeField] private GameObject prefab;

        public GameObject Prefab
        {
            get { return prefab; }
        }

        public static IVNData None
        {
            get
            {
                if (_none == null)
                {
                    _none = CreateInstance<VNExpressionData>();
                    //_none.name = "None";
                }
                return _none;
            }
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
        public IVNData NoneData
        {
            get
            {
                return None;
            }
        }
    }
}