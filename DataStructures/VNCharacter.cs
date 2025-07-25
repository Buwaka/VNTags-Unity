﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace VNTags
{
    /// <summary>
    /// VNCharacter struct containing the Name/ID, main body object, expressions and outfits,
    /// Body is the bottom layer, Outfit will be rendered as middle layer, Expression will be rendered as top layer,
    ///
    /// Fields are public for serialization purposes,
    /// please use the functions to get the data rather than directly accessing the fields
    /// </summary>
    [System.Serializable]
    public class VNCharacter
    {
        [Tooltip("Name that will be rendered, case insensitive")]
        public string Name;
        [Tooltip("Alternative names for writing convenience, case insensitive")]
        public string[] Alias;
        [Tooltip("Name for the expression, to be used as a case-insensitive ID, Expression gameobject will be attached as a child object to the VNCharacter object, first in the list is the default")]
        public VNExpression[] Expressions;
        [Tooltip("Name for the Outfit, to be used as a case-insensitive ID, Outfit gameobject will be attached as a child object to the VNCharacter object, first in the list is the default")]
        public VNOutfit[] Outfits;

        private Pair<VNOutfit, GameObject> _currentOutfit;
        private Pair<VNExpression, GameObject> _currentExpression;
        
        
        public VNExpression GetExpressionByIndex(int index)
        {
            if (index > 0 && index <= Expressions.Length)
            {
                return Expressions[index - 1];
            }

            return null;
        }
        
        public VNExpression GetExpressionByName(string name)
        {
            foreach (var expression in Expressions)
            {
                if (expression != null &&
                    String.Equals(expression.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return expression;
                }
            }

            return null;
        }
        
        public VNOutfit GetOutfitByIndex(int index)
        {
            if (index > 0 && index <= Outfits.Length)
            {
                return Outfits[index - 1];
            }

            return null;
        }
        
        public VNOutfit GetOutfitByName(string name)
        {
            foreach (var outfit in Outfits)
            {
                if (outfit != null &&
                    String.Equals(outfit.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return outfit;
                }
            }

            return null;
        }
        
        public void Init(GameObject parentObject)
        {
            if (parentObject == null)
            {
                Debug.LogError("VNCharacter: Init: provided parent is null, exiting");
                return;
            }

            if (Expressions.Length > 0 && Expressions[0] != null)
            {
                VNExpression defaultExpression = Expressions[0];
                _currentExpression = new Pair<VNExpression, GameObject>(defaultExpression,
                    GameObject.Instantiate(defaultExpression.Expression, parentObject.transform));
            }
            else
            {
                Debug.LogWarning("VNCharacter: Init: This character has no expressions or the first expression is null");
            }
            
            if (Outfits.Length > 0 && Outfits[0] != null)
            {
                VNOutfit defaultOutfit = Outfits[0];
                _currentOutfit = new Pair<VNOutfit, GameObject>(defaultOutfit,
                    GameObject.Instantiate(defaultOutfit.Outfit, parentObject.transform));
            }
            else
            {
                Debug.LogWarning("VNCharacter: Init: This character has no outfits or the first outfit is null");
            }
            
        }


#if UNITY_EDITOR
        
        /// <summary>
        /// Is used in the script editor inspector script,
        /// 0 is always null or " "
        /// </summary>
        /// <returns></returns>
        public GUIContent[] GetExpressionNamesGUI(string nullValue)
        {
            int total = Expressions.Length + 1;
            GUIContent[] result = new GUIContent[total];
            
            result[0] = new GUIContent(nullValue);
            
            for (int i = 0; i < Expressions.Length; i++)
            {
                result[i + 1] = new GUIContent(Expressions[i].Name);
            }

            return result;
        }
        
        /// <summary>
        /// Is used in the script editor inspector script,
        /// 0 is always null or " "
        /// </summary>
        /// <returns></returns>
        public GUIContent[] GetOutfitNamesGUI(string nullValue)
        {
            int total = Outfits.Length + 1;
            GUIContent[] result = new GUIContent[total];
            
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
    /// Name for the expression, to be used as a case-insensitive ID,
    /// Expression gameobject will be attached as a child object to the VNCharacter object
    /// </summary>
    [System.Serializable]
    public class VNExpression
    {
        public string Name;
        public GameObject Expression;
    }
    
    /// <summary>
    /// Name for the expression, to be used as a case-insensitive ID,
    /// Outfit gameobject will be attached as a child object to the VNCharacter object
    /// </summary>
    [System.Serializable]
    public class VNOutfit
    {
        public string Name;
        public GameObject Outfit;
    }
}