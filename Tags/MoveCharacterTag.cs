using System;
using UnityEngine;

namespace VNTags
{

    public delegate bool CharacterMoveHandler(VNTagContext context, VNCharacterData character, string position);
    
    public class MoveCharacterTag : VNTag
    {
        
        private VNCharacterData _character;
        private string _namedPosition;
        
        public override string GetTagName()
        {
            return "Move";
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished =
                VNTag.ExecuteHelper(VNTagEventAnnouncer.onCharacterMoveTag?.Invoke(context,
                                      _character,
                                      _namedPosition));
        }

        public override void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if ((parameters == null) || (parameters.Length <= 1))
            {
                Debug.LogError("MoveCharacterTag: Deserialize: Not enough parameters provided '" + context + "'");
                return;
            }
            
            _character     = VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(parameters[0]);
            _namedPosition = parameters[1];

            if (_character == null)
            {
                Debug.LogError("MoveCharacterTag: Deserialize: Failed to find Character with name '"
                             + parameters[0]
                             + "', "
                             + context);
            }
            
            if (string.IsNullOrEmpty(_namedPosition))
            {
                Debug.LogError("MoveCharacterTag: Deserialize: provided position is empty or null '"
                             + parameters[1]
                             + "', "
                             + context);
            }
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            return _character != null ? VNTag.SerializeHelper(GetTagName(), _character.Name, _namedPosition) : "";
        }
    }
}