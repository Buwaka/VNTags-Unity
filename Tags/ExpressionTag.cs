using UnityEngine;

namespace VNTags
{
    public delegate bool ExpressionHandler(VNTagContext context, VNCharacter character, VNExpression expression);
    public class ExpressionTag : IVNTag
    {
        private VNCharacter _targetCharacter;
        private VNExpression _expression;

        public VNCharacter TargetCharacter => _targetCharacter;
        public VNExpression Expression => _expression;

        public void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if (parameters.Length >= 2)
            {
                // character ref
                _targetCharacter = VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(parameters[0]);
                if (TargetCharacter == null)
                {
                    Debug.LogError("ExpressionTag: Deserialize: failed to find character name, " + parameters[0] + ", context: " + context);
                    return;
                }
                
                //outfit ref
                _expression = TargetCharacter.GetExpressionByName(parameters[1]);
                if (Expression == null)
                {
                    Debug.LogError("ExpressionTag: Deserialize: failed to find corresponding Expression name, " + parameters[0] + ", context: " + context);
                    return;
                }
            }
            else
            {
                Debug.LogError("ExpressionTag: Deserialize: failed to deserialize, not enough parameters, " + parameters + ", context: " + context);
            }
        }

        public string Serialize(VNTagSerializationContext context)
        {
            if (_targetCharacter == null)
            {
                _targetCharacter = context.GetMainCharacter();
            }
            
            return _expression != null && _targetCharacter != null ?  IVNTag.SerializeHelper(GetTagID(), _targetCharacter.Name, _expression.Name) : "";
        }

        public string GetTagID()
        {
            return "Expression";
        }
        
#if UNITY_EDITOR
        public ref VNExpression GetOutfitRef()
        {
            return ref _expression;
        }
#endif

        public void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = IVNTag.ExecuteHelper(VNTagEventAnnouncer.onExpressionChange?.Invoke(context, TargetCharacter, Expression));
        }
    }
}