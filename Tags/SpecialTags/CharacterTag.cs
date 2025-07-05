using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VNTags
{
    public class CharacterTag : IVNTag
    {
        public string RawString = "";
        public VNCharacter Character;

        public void Deserialize(string name, VNTagLineContext context)
        {
            RawString = name;
            Character = VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(name);

            if (Character == null)
            {
                Debug.LogError("CharacterTag: Init: Failed to find Character with name '" + name + "', " + context);
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
