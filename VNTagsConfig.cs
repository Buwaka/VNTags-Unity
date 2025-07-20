using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VNTags
{

    [CreateAssetMenu(fileName = "VNTagsConfig", menuName = "ScriptableObjects/VNTagsConfig")]
    public class VNTagsConfig : ScriptableObject
    {
        [SerializeField]
        public VNCharacter[] Characters;
        
        [SerializeField]
        public VNBackground[] Backgrounds;
        
        [SerializeField]
        public VNSound[] SoundEffects;
        
        [SerializeField]
        public VNMusic[] Musics;
        
        [SerializeField]
        public VNScene[] Scenes;


        public VNCharacter GetCharacterByNameOrAlias(string CharacterName)
        {
            foreach (var character in Characters)
            {
                if (character.Name.Equals(CharacterName, StringComparison.OrdinalIgnoreCase) || 
                    character.Alias.Any(chara => chara.Equals(CharacterName, StringComparison.OrdinalIgnoreCase)))
                {
                    return character;
                }
            }

            return null;
        }
        
        public VNCharacter GetCharacterByIndex(int index)
        {
            if (index > 0 && index <= Characters.Length)
            {
                return Characters[index - 1];
            }

            return null;
        }



        private static VNTagsConfig config = null;
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
#endif
        }
        
        
#if UNITY_EDITOR
        /// <summary>
        /// Is used in the script editor inspector script,
        /// 0 is always nullValue
        /// </summary>
        /// <returns></returns>
        public GUIContent[] GetCharacterNamesGUI(string nullValue)
        {
            int total = Characters.Length + 1;
            GUIContent[] result = new GUIContent[total];
            
            result[0] = new GUIContent(nullValue);
            
            for (int i = 0; i < Characters.Length; i++)
            {
                result[i + 1] = new GUIContent(Characters[i].Name);
            }

            return result;
        }
        
        /// <summary>
        /// Is used in the script editor inspector script,
        /// 0 is always nullValue
        /// </summary>
        /// <returns></returns>
        public GUIContent[] GetBackgroundNamesGUI(string nullValue)
        {
            int total = Backgrounds.Length + 1;
            GUIContent[] result = new GUIContent[total];
            
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