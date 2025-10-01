using System;
using UnityEngine;

namespace VNTags.Tags
{
    public delegate bool DialogueHandler(VNTagContext context, string dialogue);

    public class DialogueTag : VNTag
    {
        private string _processedDialogue;
        public  string Dialogue { get; private set; } = "";

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if ((parameters == null) || (parameters.Length <= 0))
            {
                Debug.LogError("DialogueTag: Deserialize: No parameters provided '" + context + "'");
                return false;
            }

            Dialogue = parameters[0];
            return true;
        }

        protected override VNTagParameters Parameters(VNTagParameters currentParameters)
        {
            var dialogueParameter = new VNTagParameter(1, "Dialogue", TypeCode.String, "Text to be rendered as dialogue");
            currentParameters.UpdateParameter(dialogueParameter, Dialogue);
            return currentParameters;
        }

        public override bool EditorVisibility()
        {
            return false;
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            return Dialogue;
        }

        public override string GetTagName()
        {
            return "";
        }

        public void Refresh()
        {
            _processedDialogue = TextProcessors.TextProcessors.PreProcessDialogue(Dialogue);
            _processedDialogue = TextProcessors.TextProcessors.PostProcessDialogue(_processedDialogue);
        }


        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            if (_processedDialogue == null)
            {
                Refresh();
            }

            isFinished = ExecuteHelper(VNTagEventAnnouncer.onDialogueTag?.Invoke(context, _processedDialogue));
        }

#if UNITY_EDITOR
        public void SetDialogue(string dialogue)
        {
            Dialogue = dialogue;
        }
#endif
    }
}