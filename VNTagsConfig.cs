using System.Collections.Generic;
using UnityEngine;

namespace VNTags
{

    [CreateAssetMenu(fileName = "VNTagsConfig", menuName = "ScriptableObjects/VNTagsConfig")]
    public class VNTagsConfig : ScriptableObject
    {
        [SerializeField]
        public VNCharacter[] Characters;
        
        [SerializeField]
        public VNBackground[] Backgrounds;
    }
}