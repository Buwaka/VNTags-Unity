using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VNTags
{
    public class DialogueTag : IVNTag
    {
        public string Dialogue = "";

        public void Deserialize(VNTagLineContext context, params string[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
            {
                Debug.LogError("DialogueTag: Deserialize: No parameters provided '" + context + "'");
                return;
            }
            Dialogue = parameters[0];
        }

        public string Serialize()
        {
            return Dialogue;
        }

        public string GetTagID()
        {
            return "";
        }


        public void Execute(VNTagContext context, out bool isFinished)
        {
            if(context.TextBox != null)
            {
                context.TextBox.text = Dialogue;
            }
            else
            {
                Debug.LogError("DialogueTag: Execute: TextWindow in VNTagContext is null");
            }
            isFinished = true;
        }
    }
}
