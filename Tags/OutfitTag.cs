using System;
using UnityEngine;

namespace VNTags.Tags
{
    public delegate bool OutfitHandler(VNTagContext context, VNCharacterData character, VNOutfitData expression);

    public class OutfitTag : VNTag
    {
        private VNOutfitData _outfit;

        public VNCharacterData TargetCharacter { get; private set; }

        public VNOutfitData Outfit
        {
            get { return _outfit; }
        }

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if (parameters.Length >= 2)
            {
                // character ref
                TargetCharacter = VNTagsConfig.GetConfig().GetCharacterByNameOrAlias(parameters[0]);
                if (TargetCharacter == null)
                {
                    Debug.LogError("OutfitTag: Deserialize: failed to find character name, " + parameters[0] + ", context: " + context);
                    return false;
                }

                //outfit ref
                _outfit = TargetCharacter.GetOutfitByName(parameters[1]);
                if (Outfit == null)
                {
                    Debug.LogError("OutfitTag: Deserialize: failed to find corresponding outfit name, " + parameters[0] + ", context: " + context);
                    return false;
                }
            }
            else
            {
                Debug.LogError("OutfitTag: Deserialize: failed to deserialize, not enough parameters, " + parameters + ", context: " + context);
                return false;
            }

            return true;
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            if (TargetCharacter == null)
            {
                TargetCharacter = context.GetMainCharacter();
            }

            return _outfit != null && !_outfit.IsNone() && TargetCharacter != null && !TargetCharacter.IsNone()
                ? SerializeHelper(GetTagName(), TargetCharacter.Name, Outfit.Name)
                : "";
        }

        public override string GetTagName()
        {
            return "Outfit";
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
            currentParameters.TryGetValue(characterParameter, out string character);

            var outfitParameter = new VNTagParameter(2,
                                                     "Outfit",
                                                     TypeCode.String,
                                                     "Outfit to render for given character",
                                                     false,
                                                     null,
                                                     VNTagsConfig.GetConfig().GetOutfitNames(character));

            currentParameters.DefaultParameter(characterParameter, TargetCharacter?.Name!);
            currentParameters.DefaultParameter(outfitParameter,    _outfit?.Name!);

            return currentParameters;
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onOutfitTag?.Invoke(context, TargetCharacter, Outfit));
        }

#if UNITY_EDITOR
        public ref VNOutfitData GetOutfitRef()
        {
            return ref _outfit;
        }
#endif
    }
}