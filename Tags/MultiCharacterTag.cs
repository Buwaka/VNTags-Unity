using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VNTags.Tags
{
    public class MultiCharacterTag : VNTag
    {
        [SerializeField] private CharacterAction _action = 0;

        [SerializeField] private VNCharacterData[] _characters = Array.Empty<VNCharacterData>();

        public VNCharacterData[] Characters
        {
            get { return _characters; }
        }

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
            {
                Debug.LogError("CharacterTag: Deserialize: No parameters provided '" + context + "'");
                return false;
            }

            int index      = 0;
            var characters = new List<VNCharacterData>();
            while (parameters.Length > index && VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(parameters[index]) is VNCharacterData character)
            {
                characters.Add(character);
                index++;
            }

            _characters = characters.ToArray();

            if (parameters.Length > index)
            {
                if (CharacterAction.TryParse(parameters[index], true, out CharacterAction result))
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
            return _characters != null && _characters.Length > 0 ? SerializeHelper(GetTagName(), _characters.Select(t => t.Name), _action) : "";
        }

        public override string GetTagName()
        {
            return "MultiCharacter";
        }

        protected override VNTagParameters Parameters(VNTagParameters currentParameters)
        {
            // todo make VNTagParameter handle arrays
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

            currentParameters.DefaultParameter(characterParameter, string.Join(";", _characters.Select(t => t.Name)));
            currentParameters.DefaultParameter(actionParameter,    _action.ToString());

            return currentParameters;
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = true;
            foreach (VNCharacterData character in _characters)
            {
                isFinished &= ExecuteHelper(VNTagEventAnnouncer.onCharacterTag?.Invoke(context, character, _action));
            }
        }

#if UNITY_EDITOR
        public ref VNCharacterData[] GetCharacterRef()
        {
            return ref _characters;
        }
#endif
    }
}