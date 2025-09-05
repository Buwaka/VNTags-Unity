using System;
using System.Collections.Generic;
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

            return (_outfit != null) && (TargetCharacter != null) ? SerializeHelper(GetTagName(), TargetCharacter.Name, Outfit.Name) : "";
        }

        public override string GetTagName()
        {
            return "Outfit";
        }

        public override VNTagParameter[] GetParameters(IList<object> currentParameters)
        {
            string character = null;
            if (currentParameters.Count > 0)
            {
                character = (string)currentParameters[0];
            }

            return new[]
            {
                new VNTagParameter("Character",
                                   TypeCode.String,
                                   "Character to add or remove from the scene",
                                   null,
                                   false,
                                   null,
                                   VNTagsConfig.GetConfig().GetCharacterNames()),
                new VNTagParameter("Outfit",
                                   TypeCode.String,
                                   "Outfit to render for given character",
                                   null,
                                   false,
                                   null,
                                   VNTagsConfig.GetConfig().GetOutfitNames(character))
            };
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