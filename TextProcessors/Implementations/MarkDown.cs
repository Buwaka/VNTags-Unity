using UnityEngine;
using VNTags.Utility;

namespace VNTags.TextProcessors
{
    [CreateAssetMenu(fileName = "MarkDown", menuName = "VNTags/TextProcessors/MarkDown")]
    public class MarkDown : BaseTextProcessor
    {
        [SerializeField] private string header1Style       = "H1";
        [SerializeField] private string header2Style       = "H2";
        [SerializeField] private string header3Style       = "H3";
        [SerializeField] private string headerSubtextStyle = "H-";

        [SerializeField] private string boldStyle          = "Bold";
        [SerializeField] private string italicStyle        = "Italic";
        [SerializeField] private string underlineStyle     = "Underline";
        [SerializeField] private string strikethroughStyle = "Strikethrough";

        [SerializeField] private string subtextStyle   = "Sub";
        [SerializeField] private string superTextStyle = "Super";

        public override string PreProcessDialogue(string text)
        {
            string processedText = ProcessHeader(text);
            processedText = ProcessTextFormatting(processedText);
            processedText = ProcessSuperSubText(processedText);
            return processedText;
        }

        private string ProcessTextFormatting(string text)
        {
            const string italicsTag       = "*";
            const string boldTag          = "**";
            const string underlineTag     = "__";
            const string strikethroughTag = "~~";

            string processedText = text.SwapEnclosing(strikethroughTag, OpeningTag(strikethroughStyle), "</style>");
            processedText = processedText.SwapEnclosing(underlineTag, OpeningTag(underlineStyle), "</style>");
            processedText = processedText.SwapEnclosing(boldTag,      OpeningTag(boldStyle),      "</style>");
            processedText = processedText.SwapEnclosing(italicsTag,   OpeningTag(italicStyle),    "</style>");

            return processedText;
        }

        private string ProcessSuperSubText(string text)
        {
            const string superChar = "^";
            const string subChar   = "_";

            string processedText = text.EncloseNextWord(superChar, OpeningTag(superTextStyle), "</style>");
            processedText = processedText.EncloseNextWord(subChar, OpeningTag(subtextStyle), "</style>");

            return processedText;
        }

        private string ProcessHeader(string text)
        {
            const char headerChar = '#';
            const char subChar    = '-';

            char? firstChar = text.FirstChar(' ');

            if (firstChar is not (headerChar or subChar))
            {
                return text;
            }

            int  startIndex   = text.IndexOf(firstChar.Value) + 1;
            int  headerPoints = firstChar.Value == headerChar ? 1 : 0;
            bool sub          = firstChar.Value == subChar;

            while (true)
            {
                if (text[startIndex] != headerChar)
                {
                    break;
                }

                headerPoints++;
                startIndex++;
            }

            string textWithoutHeader = text.Substring(startIndex);

            // subtext header
            if (sub && (headerPoints > 0))
            {
                return OpeningTag(headerSubtextStyle) + textWithoutHeader + "</style>";
            }

            switch (headerPoints)
            {
                case 0:
                    return text;
                case 1:
                    return OpeningTag(header1Style) + textWithoutHeader + "</style>";
                case 2:
                    return OpeningTag(header2Style) + textWithoutHeader + "</style>";
                default:
                    return OpeningTag(header3Style) + textWithoutHeader + "</style>";
            }
        }

        private string OpeningTag(string style)
        {
            return "<style=\"" + style + "\">";
        }
    }
}