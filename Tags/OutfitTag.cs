using UnityEngine;

namespace VNTags
{
    public delegate bool OutfitHandler(VNTagContext context, VNCharacter character, VNOutfit expression);
    public class OutfitTag : IVNTag
    {
        private VNCharacter _targetCharacter;
        private VNOutfit _outfit;

        public VNCharacter TargetCharacter => _targetCharacter;

        public VNOutfit Outfit => _outfit;

        public void Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if (parameters.Length >= 2)
            {
                // character ref
                _targetCharacter = VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(parameters[0]);
                if (TargetCharacter == null)
                {
                    Debug.LogError("OutfitTag: Deserialize: failed to find character name, " + parameters[0] + ", context: " + context);
                    return;
                }
                
                //outfit ref
                _outfit = TargetCharacter.GetOutfitByName(parameters[1]);
                if (Outfit == null)
                {
                    Debug.LogError("OutfitTag: Deserialize: failed to find corresponding outfit name, " + parameters[0] + ", context: " + context);
                    return;
                }
            }
            else
            {
                Debug.LogError("OutfitTag: Deserialize: failed to deserialize, not enough parameters, " + parameters + ", context: " + context);
            }
        }

        public string Serialize(VNTagSerializationContext context)
        {
            if (_targetCharacter == null)
            {
                _targetCharacter = context.GetMainCharacter();
            }
            
            return _outfit != null && _targetCharacter != null ?  IVNTag.SerializeHelper(GetTagID(), TargetCharacter.Name, Outfit.Name) : "";
        }

        public string GetTagID()
        {
            return "Outfit";
        }
        
#if UNITY_EDITOR
        public ref VNOutfit GetOutfitRef()
        {
            return ref _outfit;
        }
#endif

        public void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = IVNTag.ExecuteHelper(VNTagEventAnnouncer.onOutfitChange?.Invoke(context, TargetCharacter, Outfit));
        }
    }
}