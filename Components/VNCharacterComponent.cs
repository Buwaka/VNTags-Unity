using System.Collections.Generic;
using UnityEngine;

namespace VNTags.Components
{
    public class VNCharacterComponent : MonoBehaviour
    {
        private readonly Dictionary<VNExpressionData, GameObject> _expressionObjects = new();
        private readonly Dictionary<VNOutfitData, GameObject>     _outfitObjects     = new();

        public VNCharacterData CharacterData { get; private set; }

        public VNExpressionData CurrentExpression { get; private set; }

        public VNOutfitData CurrentOutfit { get; private set; }

        public void Init(VNCharacterData characterData)
        {
            if (characterData == null)
            {
                Debug.LogError("VNCharacterComponent: Init: provided parent is null, exiting");
                return;
            }

            if (gameObject == null)
            {
                Debug.LogError("VNCharacterComponent: Init: VNCharacterComponent is not attached to an object, exiting");
                return;
            }

            CharacterData = characterData;

            if ((CurrentExpression == null) && (CharacterData.Expressions.Length > 0) && (CharacterData.Expressions[0] != null))
            {
                VNExpressionData defaultExpression = CharacterData.Expressions[0];
                CurrentExpression = defaultExpression;
            }
            else
            {
                Debug.LogWarning("VNCharacter: Init: This character has no expressions or the first expression is null");
            }

            if ((CurrentOutfit == null) && (CharacterData.Outfits.Length > 0) && (CharacterData.Outfits[0] != null))
            {
                VNOutfitData defaultOutfit = CharacterData.Outfits[0];
                CurrentOutfit = defaultOutfit;
            }
            else
            {
                Debug.LogWarning("VNCharacter: Init: This character has no outfits or the first outfit is null");
            }
        }

        public void Show(bool show = true)
        {
            if (CurrentExpression != null)
            {
                GameObject exprObj = _expressionObjects.GetValueOrDefault(CurrentExpression, Load(CurrentExpression));
                if (exprObj != null)
                {
                    exprObj.SetActive(show);
                }
            }

            if (CurrentOutfit != null)
            {
                GameObject outfObj = _outfitObjects.GetValueOrDefault(CurrentOutfit, Load(CurrentOutfit));
                if (outfObj != null)
                {
                    outfObj.SetActive(show);
                }
            }
        }

        public void Hide()
        {
            Show(false);
        }

        private GameObject Load(VNExpressionData expression, bool reload = false)
        {
            if ((expression == null) || (expression.Prefab == null))
            {
                Debug.LogError("VNCharacter: Load: expression or expression object is null, aborting");
                return null;
            }

            if (_expressionObjects.ContainsKey(expression))
            {
                if (reload)
                {
                    _expressionObjects[expression].SetActive(false);
                    Destroy(_expressionObjects[expression]);
                    _expressionObjects.Remove(expression);
                }
                else
                {
                    return _expressionObjects[expression];
                }
            }

            GameObject newExpr = Instantiate(expression.Prefab, gameObject.transform);
            _expressionObjects.Add(expression, newExpr);
            return newExpr;
        }

        private GameObject Load(VNOutfitData outfit, bool reload = false)
        {
            if ((outfit == null) || (outfit.Prefab == null))
            {
                Debug.LogError("VNCharacter: Load: outfit or outfit object is null, aborting");
                return null;
            }

            if (_outfitObjects.ContainsKey(outfit))
            {
                if (reload)
                {
                    _outfitObjects[outfit].SetActive(false);
                    Destroy(_outfitObjects[outfit]);
                    _outfitObjects.Remove(outfit);
                }
                else
                {
                    return _outfitObjects[outfit];
                }
            }

            GameObject newExpr = Instantiate(outfit.Prefab, gameObject.transform);
            _outfitObjects.Add(outfit, newExpr);
            return newExpr;
        }

        public void PreloadExpressions(params VNExpressionData[] expressions)
        {
            foreach (VNExpressionData expr in expressions)
            {
                Load(expr);
            }
        }

        public void PreloadOutfits(params VNOutfitData[] outfits)
        {
            foreach (VNOutfitData outf in outfits)
            {
                Load(outf);
            }
        }

        public void PreloadAll()
        {
            PreloadExpressions(CharacterData.Expressions);
            PreloadOutfits(CharacterData.Outfits);
        }

        public void ChangeExpression(VNExpressionData expression)
        {
            if ((expression == null) || (expression.Prefab == null))
            {
                Debug.LogError("VNCharacter: ChangeExpression: Expression is null, aborting, " + expression);
                return;
            }

            if (expression == CurrentExpression)
            {
                Debug.Log("VNCharacter: ChangeExpression: this expression is already active, aborting, " + expression);
                return;
            }

            GameObject obj = _expressionObjects.GetValueOrDefault(expression, Load(expression));
            if (obj != null)
            {
                obj.SetActive(true);
            }
            else
            {
                Debug.LogError("VNCharacter: ChangeExpression: failed to load Expression GameObject, aborting");
                return;
            }

            if (CurrentExpression != null)
            {
                _expressionObjects[CurrentExpression].SetActive(false);
            }

            CurrentExpression = expression;
        }

        public void ChangeOutfit(VNOutfitData outfit)
        {
            if ((outfit == null) || (outfit.Prefab == null))
            {
                Debug.LogError("VNCharacter: ChangeOutfit: Outfit is null, aborting, " + outfit);
                return;
            }

            GameObject obj = _outfitObjects.GetValueOrDefault(outfit, Load(outfit));
            if (obj != null)
            {
                obj.SetActive(true);
            }
            else
            {
                Debug.LogError("VNCharacter: ChangeOutfit: failed to load Expression GameObject, aborting");
                return;
            }

            if ((CurrentOutfit != null) && (CurrentOutfit != outfit))
            {
                _outfitObjects[CurrentOutfit].SetActive(false);
            }

            CurrentOutfit = outfit;
        }
    }
}