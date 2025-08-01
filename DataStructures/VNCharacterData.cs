using System;
using UnityEngine;

namespace VNTags
{
    /// <summary>
    ///     VNCharacter struct containing the Name/ID, main body object, expressions and outfits,
    ///     Body is the bottom layer, Outfit will be rendered as middle layer, Expression will be rendered as top layer,
    ///     Fields are public for serialization purposes,
    ///     please use the functions to get the data rather than directly accessing the fields
    /// </summary>
    [Serializable]
    public class VNCharacterData
    {
        [Tooltip("Name that will be rendered, case insensitive")]
        public string Name;

        [Tooltip("Alternative names for writing convenience, case insensitive")]
        public string[] Alias;

        [Tooltip(
                    "Name for the expression, to be used as a case-insensitive ID, Expression gameobject will be attached as a child object to the VNCharacter object, first in the list is the default")]
        public VNExpressionData[] Expressions;

        [Tooltip(
                    "Name for the Outfit, to be used as a case-insensitive ID, Outfit gameobject will be attached as a child object to the VNCharacter object, first in the list is the default")]
        public VNOutfitData[] Outfits;


        private VNExpressionData _defaultExpression;
        private VNOutfitData     _defaultOutfit;

        public VNExpressionData DefaultExpression
        {
            get
            {
                if ((_defaultExpression == null) && (Expressions[0] != null))
                {
                    _defaultExpression = Expressions[0];
                }

                return _defaultExpression;
            }
            set { _defaultExpression = value; }
        }

        public VNOutfitData DefaultOutfit
        {
            get
            {
                if ((_defaultOutfit == null) && (Outfits[0] != null))
                {
                    _defaultOutfit = Outfits[0];
                }

                return _defaultOutfit;
            }
            set { _defaultOutfit = value; }
        }


        public VNExpressionData GetExpressionByIndex(int index)
        {
            if ((index > 0) && (index <= Expressions.Length))
            {
                return Expressions[index - 1];
            }

            return null;
        }

        public VNExpressionData GetExpressionByName(string name)
        {
            foreach (VNExpressionData expression in Expressions)
            {
                if ((expression != null) && string.Equals(expression.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return expression;
                }
            }

            return null;
        }

        public VNOutfitData GetOutfitByIndex(int index)
        {
            if ((index > 0) && (index <= Outfits.Length))
            {
                return Outfits[index - 1];
            }

            return null;
        }

        public VNOutfitData GetOutfitByName(string name)
        {
            foreach (VNOutfitData outfit in Outfits)
            {
                if ((outfit != null) && string.Equals(outfit.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return outfit;
                }
            }

            return null;
        }


#if UNITY_EDITOR

        /// <summary>
        ///     Is used in the script editor inspector script,
        ///     0 is always null or " "
        /// </summary>
        /// <returns></returns>
        public GUIContent[] GetExpressionNamesGUI(string nullValue)
        {
            int total  = Expressions.Length + 1;
            var result = new GUIContent[total];

            result[0] = new GUIContent(nullValue);

            for (int i = 0; i < Expressions.Length; i++)
            {
                result[i + 1] = new GUIContent(Expressions[i].Name);
            }

            return result;
        }

        /// <summary>
        ///     Is used in the script editor inspector script,
        ///     0 is always null or " "
        /// </summary>
        /// <returns></returns>
        public GUIContent[] GetOutfitNamesGUI(string nullValue)
        {
            int total  = Outfits.Length + 1;
            var result = new GUIContent[total];

            result[0] = new GUIContent(nullValue);

            for (int i = 0; i < Outfits.Length; i++)
            {
                result[i + 1] = new GUIContent(Outfits[i].Name);
            }

            return result;
        }
#endif
    }

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

    /// <summary>
    ///     Name for the expression, to be used as a case-insensitive ID,
    ///     Outfit gameobject will be attached as a child object to the VNCharacter object
    /// </summary>
    [Serializable]
    public class VNOutfitData
    {
        public string Name;

        [Tooltip("Alternative names for writing convenience, case insensitive")]
        public string[] Alias;

        public GameObject Object;
    }
}