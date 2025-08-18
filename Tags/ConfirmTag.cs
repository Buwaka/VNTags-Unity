using System;
using System.Linq.Expressions;
using Serialize.Linq.Serializers;
using UnityEngine;

namespace VNTags.Tags
{
    using Condition = Func<bool>;

    public class ConfirmTag : VNTag
    {
        private static readonly ExpressionSerializer _serializer = new(new JsonSerializer());


        private       Condition _conditionOverride;
        public static Condition DefaultCondition { get; set; } = null;


        public override void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            // only when there is an extra condition provided, otherwise it will just use the default condition
            if ((parameters != null) && (parameters.Length > 0))
            {
                try
                {
                    string conditionData = parameters[0];
                    var    expr          = (Expression<Func<bool>>)_serializer.DeserializeText(conditionData);
                    _conditionOverride = expr.Compile();
                }
                catch (Exception e)
                {
                    Debug.LogError("ConfirmTag: Deserialize: failed to deserialize condition, '"
                                 + context
                                 + "', exception: "
                                 + e);
                }
            }
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            if (_conditionOverride == null)
            {
                return SerializeHelper(GetTagName());
            }

            Expression<Func<bool>> exprCondition       = () => _conditionOverride.Invoke();
            string                 serializedCondition = _serializer.SerializeText(exprCondition);

            return SerializeHelper(GetTagName(), serializedCondition);
        }

        public override string GetTagName()
        {
            return "Confirm";
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            if (_conditionOverride != null)
            {
                isFinished = _conditionOverride.Invoke();
            }
            else
            {
                isFinished = DefaultCondition?.Invoke() ?? true;
            }
        }
    }
}