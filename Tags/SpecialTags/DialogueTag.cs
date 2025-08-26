using UnityEngine;
using VNTags.TextProcessors;

namespace VNTags.Tags
{
    public delegate bool DialogueHandler(VNTagContext context, string dialogue);

    public class DialogueTag : VNTag
    {
        public string Dialogue { get; private set; } = "";

        public override void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if ((parameters == null) || (parameters.Length <= 0))
            {
                Debug.LogError("DialogueTag: Deserialize: No parameters provided '" + context + "'");
                return;
            }

            Dialogue = parameters[0];
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            return Dialogue;
        }

        public override string GetTagName()
        {
            return "";
        }


        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            string processedDialogue = TextProcessors.TextProcessors.PreProcessDialogue(Dialogue);
            processedDialogue = TextProcessors.TextProcessors.PostProcessDialogue(processedDialogue);
            
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onDialogueTag?.Invoke(context, processedDialogue));
            // if(context.TextBox != null)
            // {
            //     context.TextBox.text = Dialogue;
            // }
            // else
            // {
            //     Debug.LogError("DialogueTag: Execute: TextWindow in VNTagContext is null");
            // }
            // isFinished = true;
        }

#if UNITY_EDITOR
        public void SetDialogue(string dialogue)
        {
            Dialogue = dialogue;
        }
#endif
    }
}