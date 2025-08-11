using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VNTags
{
    [CreateAssetMenu(fileName = "VNTagsConfig", menuName = "ScriptableObjects/VNTagsConfig")]
    public class VNTagsConfig : ScriptableObject
    {
        private static VNTagsConfig config;

        [SerializeField] public VNCharacterData[] Characters;

        [SerializeField] public VNBackgroundData[] Backgrounds;

        [SerializeField] public VNSoundData[] SoundEffects;

        [SerializeField] public VNMusicData[] Musics;

        [SerializeField] public VNScene[] Scenes;


        public VNCharacterData GetCharacterByNameOrAlias(string CharacterName)
        {
            foreach (VNCharacterData character in Characters)
            {
                if (character.Name.Equals(CharacterName, StringComparison.OrdinalIgnoreCase)
                 || character.Alias.Any(chara => chara.Equals(CharacterName, StringComparison.OrdinalIgnoreCase)))
                {
                    return character;
                }
            }

            return null;
        }

        public VNCharacterData GetCharacterByIndex(int index)
        {
            if ((index > 0) && (index <= Characters.Length))
            {
                return Characters[index - 1];
            }

            return null;
        }

        public static VNTagsConfig GetConfig()
        {
            if (config)
            {
                return config;
            }

#if UNITY_EDITOR
            // 1. Use AssetDatabase.FindAssets with a type filter
            // The filter string uses "t:" followed by the type name.
            // For ScriptableObjects, use the class name.
            string[] guids = AssetDatabase.FindAssets("t:VNTagsConfig");

            if (guids.Length == 0)
            {
                Debug.LogWarning("No VNTagsConfig asset found in the project.");
                return null;
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);

            // 2. Load the asset using its path
            config = AssetDatabase.LoadAssetAtPath<VNTagsConfig>(assetPath);

            return config;
#else
            // todo how to access config at runtime
                    // Load the ScriptableObject from a Resources folder at runtime
        config = Resources.Load<VNTagsConfig>("VNTagsConfig");

        if (config == null)
        {
            Debug.LogError("VNTagsConfig asset not found in any 'Resources' folder. Please ensure the asset is placed in a folder named 'Resources' and that its file name matches 'VNTagsConfig'.");
        }

        return config;
#endif
        }


#if UNITY_EDITOR
        /// <summary>
        ///     Is used in the script editor inspector script,
        ///     0 is always nullValue
        /// </summary>
        /// <returns></returns>
        public GUIContent[] GetCharacterNamesGUI(string nullValue)
        {
            int total  = Characters.Length + 1;
            var result = new GUIContent[total];

            result[0] = new GUIContent(nullValue);

            for (int i = 0; i < Characters.Length; i++)
            {
                result[i + 1] = new GUIContent(Characters[i].Name);
            }

            return result;
        }

        /// <summary>
        ///     Is used in the script editor inspector script,
        ///     0 is always nullValue
        /// </summary>
        /// <returns></returns>
        public GUIContent[] GetBackgroundNamesGUI(string nullValue)
        {
            int total  = Backgrounds.Length + 1;
            var result = new GUIContent[total];

            result[0] = new GUIContent(nullValue);

            for (int i = 0; i < Backgrounds.Length; i++)
            {
                result[i + 1] = new GUIContent(Backgrounds[i].Name);
            }

            return result;
        }
#endif
    }
}