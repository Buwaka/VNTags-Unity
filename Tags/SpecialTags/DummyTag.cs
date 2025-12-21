using UnityEngine;

namespace VNTags.Tags
{
    public class DummyTag : VNTag, IEditorTag
    {
        public string TagString = "";

        public string GetValue()
        {
            return TagString;
        }

        public void SetValue(string value)
        {
            TagString = value;
        }
        
        public VNTagID GetID()
        {
            return ID;
        }

        public override string GetTagName()
        {
            return "Dummy";
        }

        protected override VNTagParameters Parameters(VNTagParameters currentParameters)
        {
            return new VNTagParameters();
        }

        public override bool EditorVisible()
        {
            return false;
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = true;
        }

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if ((parameters == null) || (parameters.Length <= 0))
            {
                Debug.LogError("DialogueTag: Deserialize: No parameters provided '" + context + "'");
                return false;
            }

            TagString = parameters[0];
            return true;
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            return TagString;
        }
    }
}