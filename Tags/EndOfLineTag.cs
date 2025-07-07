using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VNTags
{
    public class EndOfLineTag : IVNTag
    {

        public EndOfLineTag()
        {
        }

        public void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            
        }

        public string Serialize(VNTagSerializationContext context)
        {
            return String.Empty;
        }

        public string GetTagID()
        {
            return "EOL";
        }


        public void Execute(VNTagContext context, out bool isFinished)
        {
            if(context.TextBox != null)
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
