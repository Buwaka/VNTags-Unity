using UnityEngine;

namespace VNTags
{
    public delegate bool DialogueHandler(VNTagContext context, string dialogue);

    public class DialogueTag : IVNTag
    {
        public string Dialogue { get; private set; } = "";

        public void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if ((parameters == null) || (parameters.Length <= 0))
            {
                Debug.LogError("DialogueTag: Deserialize: No parameters provided '" + context + "'");
                return;
            }

            Dialogue = parameters[0];
        }

        public string Serialize(VNTagSerializationContext context)
        {
            return Dialogue;
        }

        public string GetTagID()
        {
            return "";
        }


        public void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = IVNTag.ExecuteHelper(VNTagEventAnnouncer.onDialogueChange?.Invoke(context, Dialogue));
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