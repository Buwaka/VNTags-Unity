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

        public void Deserialize(string parameters, VNTagLineContext context)
        {
            Dialogue = parameters;
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
