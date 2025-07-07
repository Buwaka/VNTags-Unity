using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VNTags
{
    public delegate bool DialogueHandler(VNTagContext context, string dialogue);
    public class DialogueTag : IVNTag
    {
        private string _dialogue = "";

        public string Dialogue => _dialogue;

        public void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
            {
                Debug.LogError("DialogueTag: Deserialize: No parameters provided '" + context + "'");
                return;
            }
            _dialogue = parameters[0];
        }

        public string Serialize(VNTagSerializationContext context)
        {
            return Dialogue;
        }

        public string GetTagID()
        {
            return "";
        }
        
#if UNITY_EDITOR
        public void SetDialogue(string dialogue)
        {
            _dialogue = dialogue;
        }
#endif


        public void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = IVNTag.ExecuteHelper(VNTagEventAnnouncer.onDialogueChange?.Invoke(context, _dialogue));
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
    }
}
