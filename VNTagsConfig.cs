using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VNTags.TextProcessors;


namespace VNTags
{
    [CreateAssetMenu(fileName = "VNTagsConfig", menuName = "ScriptableObjects/VNTagsConfig")]
    public class VNTagsConfig : ScriptableObject
    {
        private static VNTagsConfig config;

#pragma warning disable 0414
        [SerializeField] private string ConfigName = "VNTagsConfig";
#pragma warning restore 0414
        
        [SerializeField] private VNCharacterData[] Characters;

        [SerializeField] private VNBackgroundData[] Backgrounds;

        [SerializeField] private VNSoundData[] SoundEffects;

        [SerializeField] private VNMusicData[] Musics;

        [SerializeField] private VNTransition[] Transitions;

        [SerializeField] private VNScene[] Scenes;
        
        [SerializeReference]
        public BaseTextProcessor[] TextProcessors; // do mind this is a SerializeReference, which is necessary for polymorphism
        
        

        public VNCharacterData[] AllCharacters
        {
            get { return Characters; }
        }

        public VNBackgroundData[] AllBackgrounds
        {
            get { return Backgrounds; }
        }

        public VNSoundData[] AllSoundEffects
        {
            get { return SoundEffects; }
        }

        public VNMusicData[] AllMusics
        {
            get { return Musics; }
        }

        public VNScene[] AllScenes
        {
            get { return Scenes; }
        }

        public VNTransition[] AllTransitions
        {
            get { return Transitions; }
        }


        private IVNData GetDataByNameOrAlias(IReadOnlyList<IVNData> arr, string name)
        {
            if (name == null)
            {
                return null;
            }
            
            foreach (IVNData character in arr)
            {
                if (character.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                 || character.Alias.Any(chara => chara.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    return character;
                }
            }

            return null;
        }

        private IVNData GetDataByIndex(IReadOnlyList<IVNData> arr, int index)
        {
            if ((index > 0) && (index <= arr.Count))
            {
                return arr[index - 1];
            }

            return null;
        }

        public VNCharacterData GetCharacterByNameOrAlias(string name)
        {
            return (VNCharacterData)GetDataByNameOrAlias(Characters, name);
        }

        public VNCharacterData GetCharacterByIndex(int index)
        {
            return (VNCharacterData)GetDataByIndex(Characters, index);
        }

        public VNBackgroundData GetBackgroundByNameOrAlias(string name)
        {
            return (VNBackgroundData)GetDataByNameOrAlias(Backgrounds, name);
        }

        public VNBackgroundData GetBackgroundByIndex(int index)
        {
            return (VNBackgroundData)GetDataByIndex(Backgrounds, index);
        }

        public VNSoundData GetSoundByNameOrAlias(string name)
        {
            return (VNSoundData)GetDataByNameOrAlias(SoundEffects, name);
        }

        public VNSoundData GetSoundByIndex(int index)
        {
            return (VNSoundData)GetDataByIndex(SoundEffects, index);
        }

        public VNMusicData GetMusicByNameOrAlias(string name)
        {
            return (VNMusicData)GetDataByNameOrAlias(Musics, name);
        }

        public VNMusicData GetMusicByIndex(int index)
        {
            return (VNMusicData)GetDataByIndex(Musics, index);
        }

        public VNScene GetSceneByName(string name)
        {
            return (VNScene)GetDataByNameOrAlias(Scenes, name);
        }

        public VNScene GetSceneByIndex(int index)
        {
            return (VNScene)GetDataByIndex(Scenes, index);
        }

        public VNTransition GetTransitionByNameOrAlias(string name)
        {
            return (VNTransition)GetDataByNameOrAlias(Transitions, name);
        }

        public VNTransition GetTransitionByIndex(int index)
        {
            return (VNTransition)GetDataByIndex(Transitions, index);
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
        config = Resources.Load<VNTagsConfig>(ConfigName);

        if (config == null)
        {
            Debug.LogError("VNTagsConfig asset not found in any 'Resources' folder. Please ensure the asset is placed in a folder named 'Resources' and that its file name matches 'VNTagsConfig'.");
        }

        return config;
#endif
        }


#if UNITY_EDITOR
        public string[] GetCharacterNames()
        {
            return  Characters.Select((t) => t.Name).ToArray();
        }
        
        public string[] GetOutfitNames(string name)
        {
            var character = GetCharacterByNameOrAlias(name);
            return character?.Outfits.Select((t) => t.Name).ToArray();
        }
        
        public string[] GeExpressionNames(string name)
        {
            var character = GetCharacterByNameOrAlias(name);
            return character?.Expressions.Select((t) => t.Name).ToArray();
        }
        
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