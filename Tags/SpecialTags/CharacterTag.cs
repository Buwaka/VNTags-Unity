using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace VNTags
{
    public delegate bool CharacterHandler(VNTagContext context, VNCharacter character);
    public class CharacterTag : IVNTag
    {
        private string _rawString = "";
        private VNCharacter _character;

        public VNCharacter Character => _character;
        public string RawString => _rawString;

        public void Deserialize(VNTagDeserializationContext context, params string[] Parameters)
        {
            if (Parameters == null || Parameters.Length <= 0)
            {
                Debug.LogError("CharacterTag: Deserialize: No parameters provided '" + context + "'");
                return;
            }
            
            _rawString = Parameters[0];
            _character = VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(Parameters[0]);

            if (Character == null)
            {
                Debug.LogError("CharacterTag: Deserialize: Failed to find Character with name '" + Parameters + "', " + context);
            }
        }

        public string Serialize(VNTagSerializationContext context)
        {
            return Character != null ?  IVNTag.SerializeHelper(GetTagID(), Character.Name) : "";
        }

        public string GetTagID()
        {
            return "Character";
        }

#if UNITY_EDITOR
        public void SetCharacter(VNCharacter character)
        {
            _character = character;
        }
        
        public ref VNCharacter GetCharacterRef()
        {
            return ref _character;
        }
#endif
        public void Execute(VNTagContext context, out bool isFinished)
        {
            
            isFinished = IVNTag.ExecuteHelper(VNTagEventAnnouncer.onCharacterChange?.Invoke(context, Character));
            // if (context.CharacterNameBox == null)
            // {
            //     Debug.LogError("CharacterTag: Execute: No Character Namebox present in VNTagContext");
            // }
            // else if (Character == null)
            // {
            //     Debug.LogError("CharacterTag: Execute: No Character present in CharacterTag, '" + RawString + "'");
            // }
            // else
            // {
            //     context.CharacterNameBox.text = Character.Name;
            // }
            // isFinished = true;
        }
    }
}
