using UnityEngine;

namespace VNTags.Tags
{
    public delegate bool ExpressionHandler(VNTagContext     context,
                                           VNCharacterData  character,
                                           VNExpressionData expression);

    public class ExpressionTag : VNTag
    {
        private VNExpressionData _expression;

        public VNCharacterData TargetCharacter { get; private set; }

        public VNExpressionData Expression
        {
            get { return _expression; }
        }

        public override void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if (parameters.Length >= 2)
            {
                // character ref
                TargetCharacter = VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(parameters[0]);
                if (TargetCharacter == null)
                {
                    Debug.LogError("ExpressionTag: Deserialize: failed to find character name, "
                                 + parameters[0]
                                 + ", context: "
                                 + context);
                    return;
                }

                //outfit ref
                _expression = TargetCharacter.GetExpressionByName(parameters[1]);
                if (Expression == null)
                {
                    Debug.LogError("ExpressionTag: Deserialize: failed to find corresponding Expression name, "
                                 + parameters[0]
                                 + ", context: "
                                 + context);
                }
            }
            else
            {
                Debug.LogError("ExpressionTag: Deserialize: failed to deserialize, not enough parameters, "
                             + parameters
                             + ", context: "
                             + context);
            }
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            if (TargetCharacter == null)
            {
                TargetCharacter = context.GetMainCharacter();
            }

            return (_expression != null) && (TargetCharacter != null)
                ? SerializeHelper(GetTagName(), TargetCharacter.Name, _expression.Name)
                : "";
        }

        public override string GetTagName()
        {
            return "Expression";
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished =
                ExecuteHelper(
                              VNTagEventAnnouncer.onExpressionTag?.Invoke(context,
                                                                          TargetCharacter,
                                                                          Expression));
        }

#if UNITY_EDITOR
        public ref VNExpressionData GetOutfitRef()
        {
            return ref _expression;
        }
#endif
    }
}