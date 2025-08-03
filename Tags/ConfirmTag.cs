using UnityEngine;

namespace VNTags
{
    public class ConfirmTag : VNTag
    {
        public override void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            // todo
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            return VNTag.SerializeHelper(GetTagName());
        }

        public override string GetTagName()
        {
            return "Confirm";
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            // todo find a better way to proceed
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                isFinished = true;
            }
            else
            {
                isFinished = false;
            }
        }
    }
}