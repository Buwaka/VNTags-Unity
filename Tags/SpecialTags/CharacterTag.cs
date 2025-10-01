using System;
using UnityEngine;

namespace VNTags.Tags
{
    public delegate bool CharacterHandler(VNTagContext context, VNCharacterData character, CharacterAction action);

    public enum CharacterAction
    {
        AddedToScene,
        RemovedFromScene
    }

    public class CharacterTag : VNTag
    {
        [SerializeField] private CharacterAction _action = 0;

        [SerializeField] private VNCharacterData _character;

        public VNCharacterData Character
        {
            get { return _character; }
        }

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if ((parameters == null) || (parameters.Length <= 0))
            {
                Debug.LogError("CharacterTag: Deserialize: No parameters provided '" + context + "'");
                return false;
            }

            _character = VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(parameters[0]);

            if (Character == null)
            {
                Debug.Log("CharacterTag: Deserialize: Failed to find Character with name '" + parameters[0] + "', using only name instead " + context);
                _character = VNCharacterData.BlankCharacter(parameters[0]);
            }

            if (parameters.Length > 1)
            {
                if (CharacterAction.TryParse(parameters[1], true, out CharacterAction result))
                {
                    _action = result;
                }
                else
                {
                    Debug.LogWarning("CharacterTag: Deserialize: Failed to parse CharacterAction '"
                                   + parameters[1]
                                   + "', default action will be used, "
                                   + context);
                }
            }

            return true;
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            return Character != null ? SerializeHelper(GetTagName(), Character.Name, _action) : "";
        }

        public override string GetTagName()
        {
            return "Character";
        }

        protected override VNTagParameters Parameters(VNTagParameters currentParameters)
        {
            var characterParameter = new VNTagParameter(1,
                                                        "Character",
                                                        TypeCode.String,
                                                        "Character to add or remove from the scene",
                                                        false,
                                                        null,
                                                        VNTagsConfig.GetConfig().GetCharacterNames());
            var actionParameter = new VNTagParameter(2,
                                                     "Character Action",
                                                     TypeCode.String,
                                                     "enum value of 'add' or 'remove', will be passed along the onCharacterTag event (default is 'add')",
                                                     true,
                                                     typeof(CharacterAction));

            currentParameters.UpdateParameter(characterParameter, _character);
            currentParameters.UpdateParameter(actionParameter,    _action);

            return currentParameters;
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onCharacterTag?.Invoke(context, Character, _action));
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