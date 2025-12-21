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
    public class VNCharacterData : IVNData
    {
        [Tooltip("Name that will be rendered, case insensitive")] [SerializeField]
        private string name;

        [Tooltip("Alternative names for writing convenience, case insensitive")] [SerializeField]
        private string[] alias;

        [Tooltip("Optional but useful field, can be used for the CharacterNameColor TextProcessor")] [SerializeField]
        private Color mainColor;

        [Tooltip("Name for the expression, to be used as a case-insensitive ID, Expression gameobject will be attached as a child object to the VNCharacter object, first in the list is the default")]
        [SerializeField]
        private VNExpressionData[] expressions;

        [Tooltip("Name for the Outfit, to be used as a case-insensitive ID, Outfit gameobject will be attached as a child object to the VNCharacter object, first in the list is the default")]
        [SerializeField]
        private VNOutfitData[] outfits;

        private VNExpressionData _defaultExpression;
        private VNOutfitData     _defaultOutfit;

        public VNExpressionData[] Expressions
        {
            get { return expressions; }
        }

        public VNOutfitData[] Outfits
        {
            get { return outfits; }
        }

        public VNExpressionData DefaultExpression
        {
            get
            {
                if ((_defaultExpression == null) && (expressions[0] != null))
                {
                    _defaultExpression = expressions[0];
                }

                return _defaultExpression;
            }
            set { _defaultExpression = value; }
        }

        public VNOutfitData DefaultOutfit
        {
            get
            {
                if ((_defaultOutfit == null) && (outfits[0] != null))
                {
                    _defaultOutfit = outfits[0];
                }

                return _defaultOutfit;
            }
            set { _defaultOutfit = value; }
        }

        public Color Color
        {
            get { return mainColor; }
        }


        public string Name
        {
            get { return name; }
        }

        public string NameProcessed
        {
            get
            {
                var pName = TextProcessors.TextProcessors.PreProcessDialogue(name);
                return TextProcessors.TextProcessors.PostProcessDialogue(pName);
            }
        }

        public string[] Alias
        {
            get { return alias; }
        }

        public string DataType { get; } = "Character";

        public IVNData NoneData
        {
            get
            {
                return None;
            }
        }
        
        public static           IVNData    None = new VNCharacterData();

        public static VNCharacterData BlankCharacter(string name)
        {
            var chara = new VNCharacterData();
            chara.name = name;
            return chara;
        }

        public bool IsBlankCharacter()
        {
            return (Expressions == null) && (Outfits == null);
        }

        public VNExpressionData GetExpressionByIndex(int index)
        {
            if ((index > 0) && (index <= expressions.Length))
            {
                return expressions[index - 1];
            }

            return null;
        }

        public VNExpressionData GetExpressionByName(string name)
        {
            foreach (VNExpressionData expression in expressions)
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
            if ((index > 0) && (index <= outfits.Length))
            {
                return outfits[index - 1];
            }

            return null;
        }

        public VNOutfitData GetOutfitByName(string name)
        {
            foreach (VNOutfitData outfit in outfits)
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
            if (IsBlankCharacter())
            {
                return Array.Empty<GUIContent>();
            }

            int total  = expressions.Length + 1;
            var result = new GUIContent[total];

            result[0] = new GUIContent(nullValue);

            for (int i = 0; i < expressions.Length; i++)
            {
                result[i + 1] = new GUIContent(expressions[i].Name);
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
            if (IsBlankCharacter())
            {
                return Array.Empty<GUIContent>();
            }

            int total  = outfits.Length + 1;
            var result = new GUIContent[total];

            result[0] = new GUIContent(nullValue);

            for (int i = 0; i < outfits.Length; i++)
            {
                result[i + 1] = new GUIContent(outfits[i].Name);
            }

            return result;
        }
#endif
    }
}