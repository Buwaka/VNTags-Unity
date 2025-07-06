using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace VNTags
{
    public class CharacterTag : IVNTag
    {
        public string RawString = "";
        public VNCharacter Character;

        public void Deserialize(VNTagLineContext context, params string[] Parameters)
        {
            if (Parameters == null || Parameters.Length <= 0)
            {
                Debug.LogError("CharacterTag: Deserialize: No parameters provided '" + context + "'");
                return;
            }
            
            RawString = Parameters[0];
            Character = VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(Parameters[0]);

            if (Character == null)
            {
                Debug.LogError("CharacterTag: Deserialize: Failed to find Character with name '" + Parameters + "', " + context);
            }
        }

        public string Serialize()
        {
            return Character != null ?  IVNTag.SerializeHelper(GetTagID(), Character.Name) : "";
        }

        public string GetTagID()
        {
            return "Character";
        }


        public void Execute(VNTagContext context, out bool isFinished)
        {
            if (context.CharacterNameBox == null)
            {
                Debug.LogError("CharacterTag: Execute: No Character Namebox present in VNTagContext");
            }
            else if (Character == null)
            {
                Debug.LogError("CharacterTag: Execute: No Character present in CharacterTag, '" + RawString + "'");
            }
            else
            {
                context.CharacterNameBox.text = Character.Name;
            }
            isFinished = true;
        }
    }
}
