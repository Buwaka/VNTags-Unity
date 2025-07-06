using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VNTags
{
    public class ConfirmTag : IVNTag
    {
        public void Deserialize(VNTagLineContext context, params string[] parameters)
        {
            // todo
        }

        public string Serialize()
        {
            return IVNTag.SerializeHelper(GetTagID());
        }

        public string GetTagID()
        {
            return "Confirm";
        }

        public void Execute(VNTagContext context, out bool isFinished)
        {
            // todo find a better way to proceed
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                isFinished = true;
            }
            else
            {
                isFinished = false;
            }
        }
    }
}
