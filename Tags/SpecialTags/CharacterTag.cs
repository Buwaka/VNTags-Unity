using UnityEngine;

namespace VNTags
{
    public delegate bool CharacterHandler(VNTagContext context, VNCharacterData character, CharacterAction action);

    public enum CharacterAction
    {
        AddedToScene,
        RemovedFromScene
    }

    public class CharacterTag : VNTag
    {
        private VNCharacterData _character;

        public VNCharacterData Character
        {
            get { return _character; }
        }

        public override void Deserialize(VNTagDeserializationContext context, params string[] Parameters)
        {
            if ((Parameters == null) || (Parameters.Length <= 0))
            {
                Debug.LogError("CharacterTag: Deserialize: No parameters provided '" + context + "'");
                return;
            }
            
            _character = VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(Parameters[0]);

            if (Character == null)
            {
                Debug.LogError("CharacterTag: Deserialize: Failed to find Character with name '"
                             + Parameters
                             + "', "
                             + context);
            }
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            return Character != null ? VNTag.SerializeHelper(GetTagName(), Character.Name) : "";
        }

        public override string GetTagName()
        {
            return "Character";
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished =
                VNTag.ExecuteHelper(VNTagEventAnnouncer.onCharacterChange?.Invoke(context,
                                      Character,
                                      CharacterAction.AddedToScene));
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

#if UNITY_EDITOR
        public void SetCharacter(VNCharacterData character)
        {
            _character = character;
        }

        public ref VNCharacterData GetCharacterRef()
        {
            return ref _character;
        }
#endif
    }
}