using System;
using UnityEngine;

namespace VNTags
{
    /// <summary>
    ///     Name for the expression, to be used as a case-insensitive ID,
    ///     Outfit gameobject will be attached as a child object to the VNCharacter object
    /// </summary>
    [CreateAssetMenu(fileName = "VNOutfit", menuName = "VNTags/Data/VNOutfit")]
    [Serializable]
    public class VNOutfitData : ScriptableObject, IVNData
    {
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

        public string DataType { get; } = "Outfit";

        private static VNOutfitData _none;
        public IVNData NoneData
        {
            get
            {
                return None;
            }
        }
        
        public static IVNData None
        {
            get
            {
                if (_none == null)
                {
                    _none = CreateInstance<VNOutfitData>();
                    //_none.name = "None";
                }
                return _none;
            }
        }
    }
}