using UnityEngine;

namespace VNTags
{
    public delegate bool OutfitHandler(VNTagContext context, VNCharacterData character, VNOutfitData expression);

    public class OutfitTag : IVNTag
    {
        private VNOutfitData _outfit;

        public VNCharacterData TargetCharacter { get; private set; }

        public VNOutfitData Outfit
        {
            get { return _outfit; }
        }

        public void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if (parameters.Length >= 2)
            {
                // character ref
                TargetCharacter = VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(parameters[0]);
                if (TargetCharacter == null)
                {
                    Debug.LogError("OutfitTag: Deserialize: failed to find character name, "
                                 + parameters[0]
                                 + ", context: "
                                 + context);
                    return;
                }

                //outfit ref
                _outfit = TargetCharacter.GetOutfitByName(parameters[1]);
                if (Outfit == null)
                {
                    Debug.LogError("OutfitTag: Deserialize: failed to find corresponding outfit name, "
                                 + parameters[0]
                                 + ", context: "
                                 + context);
                }
            }
            else
            {
                Debug.LogError("OutfitTag: Deserialize: failed to deserialize, not enough parameters, "
                             + parameters
                             + ", context: "
                             + context);
            }
        }

        public string Serialize(VNTagSerializationContext context)
        {
            if (TargetCharacter == null)
            {
                TargetCharacter = context.GetMainCharacter();
            }

            return (_outfit != null) && (TargetCharacter != null)
                ? IVNTag.SerializeHelper(GetTagID(), TargetCharacter.Name, Outfit.Name)
                : "";
        }

        public string GetTagID()
        {
            return "Outfit";
        }

        public void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished =
                IVNTag.ExecuteHelper(VNTagEventAnnouncer.onOutfitChange?.Invoke(context, TargetCharacter, Outfit));
        }

#if UNITY_EDITOR
        public ref VNOutfitData GetOutfitRef()
        {
            return ref _outfit;
        }
#endif
    }
}