using UnityEngine;

namespace VNTags
{
    public class EndOfLineTag : VNTag
    {
        public override void Deserialize(VNTagDeserializationContext context, params string[] parameters) { }

        public override string Serialize(VNTagSerializationContext context)
        {
            return string.Empty;
        }

        public override string GetTagName()
        {
            return "EOL";
        }


        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            if (context.TextBox != null)
            {
                context.TextBox.text = "";
            }
            else
            {
                Debug.LogError("DialogueTag: Execute: TextWindow in VNTagContext is null");
            }

            isFinished = true;
        }
    }
}