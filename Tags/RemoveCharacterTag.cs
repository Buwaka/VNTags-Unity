using UnityEngine;

namespace VNTags
{
    public class RemoveCharacterTag : VNTag
    {
        private VNCharacterData _character;

        public override string GetTagName()
        {
            return "Remove";
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished =
                ExecuteHelper(VNTagEventAnnouncer.onCharacterTag?.Invoke(context,
                                                                         _character,
                                                                         CharacterAction.RemovedFromScene));
        }

        public override void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if ((parameters == null) || (parameters.Length <= 0))
            {
                Debug.LogError("RemoveCharacterTag: Deserialize: No parameters provided '" + context + "'");
                return;
            }

            _character = VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(parameters[0]);

            if (_character == null)
            {
                Debug.LogError("RemoveCharacterTag: Deserialize: Failed to find Character with name '"
                             + parameters
                             + "', "
                             + context);
            }
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            return _character != null ? SerializeHelper(GetTagName(), _character.Name) : "";
        }
    }
}