using System.Collections.Generic;
using UnityEngine;

namespace VNTags
{
    /// <summary>
    /// VNCharacter struct containing the Name/ID, main body object, expressions and outfits,
    /// Body is the bottom layer, Outfit will be rendered as middle layer, Expression will be rendered as top layer
    /// </summary>
    [System.Serializable]
    public struct VNCharacter
    {
        public string Name;
        public GameObject Body;
        public VNExpression[] Expressions;
        public VNOutfit[] Outfits;
    }

    /// <summary>
    /// Name for the expression, to be used as a case-insensitive ID,
    /// Expression gameobject will be attached as a child object to the VNCharacter object
    /// </summary>
    [System.Serializable]
    public struct VNExpression
    {
        public string Name;
        public GameObject Expression;
    }
    
    /// <summary>
    /// Name for the expression, to be used as a case-insensitive ID,
    /// Outfit gameobject will be attached as a child object to the VNCharacter object
    /// </summary>
    [System.Serializable]
    public struct VNOutfit
    {
        public string Name;
        public GameObject Outfit;
    }
}