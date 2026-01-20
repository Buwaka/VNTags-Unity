using UnityEngine;

namespace VNTags.TextProcessors
{
    [CreateAssetMenu(fileName = "DialogueCleaner", menuName = "VNTags/TextProcessors/DialogueCleaner")]
    public class DialogueCleaner : BaseTextProcessor
    {
        public bool removeLeadingSpaces   = true;
        public bool capitalizeFirstLetter = true;

        public override string PreProcessDialogue(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            if (capitalizeFirstLetter)
            {
                CapitalizeFirst(ref text);
            }

            if (removeLeadingSpaces)
            {
                text.TrimStart();
            }

            return text;
        }

        private void CapitalizeFirst(ref string text)
        {
            int firstLetterLine = -1;
            for (int i = 0; i < text.Length; i++)
            {
                if (!char.IsWhiteSpace(text[i]))
                {
                    firstLetterLine = i;
                    break;
                }
            }

            if (firstLetterLine == -1) return;

            char[] chars = text.ToCharArray();
            chars[firstLetterLine] = char.ToUpper(chars[firstLetterLine]);
            text = new string(chars);
        }
    }
}