using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VNTags
{
    class EndOfLineTag : IVNTag
    {

        public EndOfLineTag()
        {
        }
        
        public string GetTagID()
        {
            return "EOL";
        }


        public void Execute(VNTagContext context, out bool isFinished)
        {
            if(context.Text != null)
            {
                context.Text.text = "";
            }
            else
            {
                Debug.LogError("DialogueTag: Execute: TextWindow in VNTagContext is null");
            }
            isFinished = true;
        }
    }
}
