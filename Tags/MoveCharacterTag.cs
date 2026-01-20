using System;
using UnityEngine;

namespace VNTags.Tags
{
    public delegate bool CharacterMoveHandler(VNTagContext context, VNCharacterData character, string position);

    public class MoveCharacterTag : VNTag
    {
        private VNCharacterData _character;
        private string          _namedPosition;

        public override string GetTagName()
        {
            return "Move";
        }

        protected override VNTagParameters Parameters(VNTagParameters currentParameters)
        {
            var characterParameter = new VNTagParameter(1,
                                                        "Character",
                                                        TypeCode.String,
                                                        "Character to move",
                                                        false,
                                                        null,
                                                        VNTagsConfig.GetConfig().GetCharacterNames());
            var positionParameter = new VNTagParameter(2, "Position", TypeCode.String, "Name of the position to move the character to");

            currentParameters.DefaultParameter(characterParameter, _character?.Name!);
            currentParameters.DefaultParameter(positionParameter,  _namedPosition);

            return currentParameters;
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onCharacterMoveTag?.Invoke(context, _character, _namedPosition));
        }

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if (parameters == null || parameters.Length <= 1)
            {
                Debug.LogError("MoveCharacterTag: Deserialize: Not enough parameters provided '" + context + "'");
                return false;
            }

            _character     = VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(parameters[0]);
            _namedPosition = parameters[1];

            if (_character == null)
            {
                Debug.LogError("MoveCharacterTag: Deserialize: Failed to find Character with name '" + parameters[0] + "', " + context);
                return false;
            }

            if (string.IsNullOrEmpty(_namedPosition))
            {
                Debug.LogError("MoveCharacterTag: Deserialize: provided position is empty or null '" + parameters[1] + "', " + context);
                return false;
            }

            return true;
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            return _character != null && !_character.IsNone() ? SerializeHelper(GetTagName(), _character.Name, _namedPosition) : "";
        }
    }
}