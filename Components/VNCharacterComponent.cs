using System.Collections.Generic;
using UnityEngine;
using VNTags.ScriptAnimations;

namespace VNTags.Components
{
    public class VNCharacterComponent : MonoBehaviour
    {
        private readonly Dictionary<VNExpressionData, GameObject> _expressionObjects = new();
        private readonly Dictionary<VNOutfitData, GameObject>     _outfitObjects     = new();
        private          string                                   _sortingLayerName  = "Default";
        
        public           bool                                     isVisible          = false;

        public VNCharacterData CharacterData { get; private set; }

        public VNExpressionData CurrentExpression { get; private set; }

        public VNOutfitData CurrentOutfit { get; private set; }
        

        public void Init(VNCharacterData characterData, string sortingLayer)
        {
            _sortingLayerName = sortingLayer;
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

            Reset();
        }

        public void SetDefaultExpression()
        {
            if (CharacterData.Expressions.Length > 0 && CharacterData.Expressions[0] != null)
            {
                VNExpressionData defaultExpression = CharacterData.Expressions[0];
                ChangeExpression(defaultExpression);
            }
            else
            {
                Debug.LogWarning("VNCharacterComponent: SetDefaultExpression: This character has no expressions or the first expression is null");
            }
        }
        
        public void SetDefaultOutfit()
        {
            if (CharacterData.Outfits.Length > 0 && CharacterData.Outfits[0] != null)
            {
                VNOutfitData defaultOutfit = CharacterData.Outfits[0];
                ChangeOutfit(defaultOutfit);
            }
            else
            {
                Debug.LogWarning("VNCharacterComponent: SetDefaultOutfit: This character has no outfits or the first outfit is null");
            }
        }

        public void Reset()
        {
            SetDefaultExpression();
            SetDefaultOutfit();
        }

        public void Show(bool show = true, ScriptAnimation animation = null)
        {
            if (show)
            {
                Refresh();
            }
            
            if (show != isVisible)
            {
                if (animation)
                {
                    animation.Init(gameObject, this);
                    if (show)
                    {
                        gameObject.SetActive(true);
                    }
                    else
                    {
                        animation.OnStop += () => gameObject.SetActive(false);
                    }
                    animation.Play();
                }
                else
                {
                    if (show)
                    {
                        gameObject.SetActive(true);
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
            
            isVisible = show;
        }

        public void Hide(ScriptAnimation animation = null)
        {
            Show(false, animation);
        }

        public void Refresh()
        {
            if (CurrentExpression != null)
            {
                ChangeExpression(CurrentExpression);
            }

            if (CurrentOutfit != null)
            {
                ChangeOutfit(CurrentOutfit);
            }
        }

        private GameObject Load(VNExpressionData expression, bool reload = false)
        {
            if ((expression == null) || (expression.Prefab == null))
            {
                Debug.LogError("VNCharacterComponent: Load: expression or expression object is null, aborting");
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
            foreach (var sprite in newExpr.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.sortingLayerName = _sortingLayerName;
            }
            
            _expressionObjects.Add(expression, newExpr);
            return newExpr;
        }

        private GameObject Load(VNOutfitData outfit, bool reload = false)
        {
            if ((outfit == null) || (outfit.Prefab == null))
            {
                Debug.LogError("VNCharacterComponent: Load: outfit or outfit object is null, aborting");
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
            foreach (var sprite in newExpr.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.sortingLayerName = _sortingLayerName;
            }
            
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
                Debug.LogError("VNCharacterComponent: ChangeExpression: Expression is null, aborting, " + expression);
                return;
            }

            if (expression == CurrentExpression)
            {
                Debug.Log("VNCharacterComponent: ChangeExpression: this expression is already active, aborting, " + expression);
                return;
            }

            GameObject obj = _expressionObjects.GetValueOrDefault(expression, Load(expression));
            if (obj != null)
            {
                obj.SetActive(true);
            }
            else
            {
                Debug.LogError("VNCharacterComponent: ChangeExpression: failed to load Expression GameObject, aborting");
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
                Debug.LogError("VNCharacterComponent: ChangeOutfit: Outfit is null, aborting, " + outfit);
                return;
            }

            GameObject obj = _outfitObjects.GetValueOrDefault(outfit, Load(outfit));
            if (obj != null)
            {
                obj.SetActive(true);
            }
            else
            {
                Debug.LogError("VNCharacterComponent: ChangeOutfit: failed to load Expression GameObject, aborting");
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