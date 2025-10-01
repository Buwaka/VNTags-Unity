using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VNTags.Utility
{
    public static class StringUtils
    {
        /// <summary>
        ///     This function looks for that first character that isn't in the except list
        /// </summary>
        /// <param name="text"></param>
        /// <param name="except"></param>
        /// <returns></returns>
        public static char? FirstChar(this string text, params char[] except)
        {
            if ((text == null) || (text.Length <= 0))
            {
                return null;
            }

            if ((except == null) || (except.Length <= 0))
            {
                return text[0];
            }

            foreach (char chara in text)
            {
                bool exceptionFound = false;

                foreach (char entry in except)
                {
                    if (chara == entry)
                    {
                        exceptionFound = true;
                        break;
                    }
                }

                if (!exceptionFound)
                {
                    return chara;
                }
            }

            return null;
        }

        /// <summary>
        ///     This function looks for a matching tag and encloses the connected word next to it,
        ///     spaces after the tag won't match
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tag"></param>
        /// <param name="openingTag"></param>
        /// <param name="closingTag"></param>
        /// <returns></returns>
        public static string EncloseNextWord(this string text, string tag, string openingTag, string closingTag)
        {
            var options = RegexOptions.Compiled;

            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            // capturing the word that is connected to it
            string pattern = $"{Regex.Escape(tag)}(\\w+)";

            // $1 refers to the content of the first capturing group
            string replacement = $"{openingTag}$1{closingTag}";

            return Regex.Replace(text, pattern, replacement, options);
        }

        /// <summary>
        ///     Assuming there are enclosing tags already, this function swaps them with other tags
        /// </summary>
        /// <param name="text"></param>
        /// <param name="enclosingTag"></param>
        /// <param name="openingTag"></param>
        /// <param name="closingTag"></param>
        /// <returns></returns>
        public static string SwapEnclosing(this string text, string enclosingTag, string openingTag, string closingTag)
        {
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Compiled;

            string escapedTag  = Regex.Escape(enclosingTag);
            string pattern     = $"({escapedTag})(.*?)(?s)({escapedTag})";
            string replacement = $"{openingTag}$2{closingTag}";

            return Regex.Replace(text, pattern, replacement, options);
        }

        /// <summary>
        ///     Enclose all words that match the searchWord with enclosingTag,
        ///     uses Regex \b, so only full words will match, partial matches will be ignored
        /// </summary>
        /// <param name="text">The full text</param>
        /// <param name="searchWord">The word to enclose</param>
        /// <param name="openingTag">string that will be placed infront of matches</param>
        /// <param name="closingTag">string that will be placed after matches</param>
        /// <returns>the full text with the searchWord matches enclosed with enclosingTag</returns>
        public static string Enclose(this string text, string searchWord, string openingTag, string closingTag)
        {
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Compiled;

            string pattern     = @"\b("     + Regex.Escape(searchWord) + @")\b";
            string replacement = openingTag + "$1"                     + closingTag;

            return Regex.Replace(text, pattern, replacement, options);
        }

        /// <summary>
        ///     Enclose all words that match the searchWord with enclosingTag,
        ///     uses Regex \b, so only full words will match, partial matches will be ignored
        /// </summary>
        /// <param name="text">The full text</param>
        /// <param name="searchWord">The word to enclose</param>
        /// <param name="enclosingTag">string that will enclose mathes</param>
        /// <returns>the full text with the searchWord matches enclosed with enclosingTag</returns>
        public static string Enclose(this string text, string searchWord, string enclosingTag)
        {
            return Enclose(text, searchWord, enclosingTag, enclosingTag);
        }

        /// <summary>
        ///     Enclose all words that match the searchWord with enclosingTag,
        ///     uses Regex \b, so only full words will match, partial matches will be ignored.
        ///     This function accept multiple searchWord pairs for performance sake, it is/should be functionally the same as
        ///     Enclose()
        /// </summary>
        /// <param name="text"></param>
        /// <param name="wordTags"></param>
        /// <returns></returns>
        public static string EncloseBatch(this string text, IEnumerable<(string SearchWord, string OpeningTag, string ClosingTag)> wordTags)
        {
            // Create a lookup dictionary for the tags.
            var tagLookup = wordTags.ToDictionary(term => term.SearchWord, term => (term.OpeningTag, term.ClosingTag));
            return EncloseBatch(text, tagLookup);
        }

        public static string EncloseBatch(this string text, IReadOnlyDictionary<string, (string OpeningTag, string ClosingTag)> wordDict)
        {
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Compiled;


            // Build a single pattern from all search words.
            // Wrap each word in a capturing group.
            var    patterns      = wordDict.Keys.Select(word => $"\\b({Regex.Escape(word)})\\b");
            string masterPattern = string.Join("|", patterns);

            string Matcher(Match match)
            {
                string matchedWord = match.Value;

                if (wordDict.TryGetValue(matchedWord, out (string OpeningTag, string ClosingTag) tags))
                {
                    return $"{tags.OpeningTag}{matchedWord}{tags.ClosingTag}";
                }

                // If for some reason the word isn't in the dictionary,
                // return the original match to not break the string.
                return matchedWord;
            }

            return Regex.Replace(text, masterPattern, Matcher, options);
        }
    }
}