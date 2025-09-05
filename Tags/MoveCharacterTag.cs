using System;
using System.Collections.Generic;
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

        public override VNTagParameter[] GetParameters(IList<object> currentParameters)
        {
            return new[]
            {
                new VNTagParameter("Character", TypeCode.String, "Character to move", null, false, null, VNTagsConfig.GetConfig().GetCharacterNames()),
                new VNTagParameter("Position",  TypeCode.String, "Name of the position to move the character to")
            };
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onCharacterMoveTag?.Invoke(context, _character, _namedPosition));
        }

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if ((parameters == null) || (parameters.Length <= 1))
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
            return _character != null ? SerializeHelper(GetTagName(), _character.Name, _namedPosition) : "";
        }
    }
}