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
        public VNCharacter? Character;

        public void Init(string Name, VNTagLineContext context)
        {
            RawString = Name;
            Character = VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(Name);

            if (!Character.HasValue)
            {
                Debug.LogError("CharacterTag: Init: Failed to find Character with name '" + Name + "', " + context);
            }
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
            else if (!Character.HasValue)
            {
                Debug.LogError("CharacterTag: Execute: No Character present in CharacterTag, '" + RawString + "'");
            }
            else
            {
                context.CharacterNameBox.text = Character.Value.Name;
            }
            isFinished = true;
        }
    }
}
