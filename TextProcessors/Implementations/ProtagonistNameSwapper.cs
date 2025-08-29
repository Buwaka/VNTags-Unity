using System;
using UnityEngine;

namespace VNTags.TextProcessors
{
    [CreateAssetMenu(fileName = "ProtagonistNameSwapper",
                        menuName = "ScriptableObjects/TextProcessors/ProtagonistNameSwapper")]
    public class ProtagonistNameSwapper : BaseTextProcessor
    {
        private static Func<string> NameGetter;

        [Tooltip("Placeholder value that will be replaced during the script processing, do mind that this happens before any tags are processed, so be careful about what placeholder you use")]
        [SerializeField]
        private string PlaceHolderName = "####";

        public static void SetName(string name)
        {
            SetName(() => name);
        }

        public static void SetName(Func<string> name)
        {
            NameGetter = name;
        }

        public override string PostProcessRawScript(string text)
        {
            if (NameGetter == null)
            {
                Debug.LogError("ProtagonistNameSwapper: PreProcessDialogue: NameGetter is null");
                return text;
            }

            if (text == null)
            {
                Debug.LogError("ProtagonistNameSwapper: PreProcessDialogue: text is null");
                return null;
            }

            return text.Replace(PlaceHolderName, NameGetter.Invoke());
        }
    }
}