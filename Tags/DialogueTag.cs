using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VNTags
{
    class DialogueTag : IVNTag
    {
        string Dialogue = "";

        public DialogueTag(string dialogue)
        {
            Dialogue = dialogue;
        }
        
        public string GetTagID()
        {
            return "";
        }


        public void Execute(VNTagContext context, out bool isFinished)
        {
            if(context.Text != null)
            {
                context.Text.text = Dialogue;
            }
            else
            {
                Debug.LogError("DialogueTag: Execute: TextWindow in VNTagContext is null");
            }
            isFinished = true;
        }
    }
}
