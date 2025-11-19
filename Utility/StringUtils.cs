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
            if (text == null || text.Length <= 0)
            {
                return null;
            }

            if (except == null || except.Length <= 0)
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

            string pattern     = @"\b(" + Regex.Escape(searchWord) + @")\b";
            string replacement = openingTag + "$1" + closingTag;

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

        /// <summary>
        ///     Extracts the value in-between quotes (" and ') starting from the first quote and ending with the last quote
        ///     example:
        ///     He said "what if I don't get "first place"? huh? what then? Will the 'Grim Reaper' come and visit?"
        ///     result:
        ///     what if I don't get "first place"? huh? what then? Will the 'Grim Reaper' come and visit?
        ///     WARNING: does not check whether the first or last quote is \escaped
        /// </summary>
        /// <param name="input"></param>
        /// <param name="startIndex">optional, from where to start looking</param>
        /// <param name="endIndex">optional, where to end looking</param>
        /// <returns>
        ///     the value inside the first pair of quotes (either ' or "), if no quotes are present, the input will be
        ///     returned
        /// </returns>
        public static string ExtractEnclosing(this string input, int startIndex = 0, int endIndex = -1)
        {
            if (endIndex == -1)
            {
                endIndex = input.Length - startIndex;
            }

            int doubleQuoteStartIndex = input.IndexOf('"',  startIndex, endIndex - startIndex);
            int singleQuoteStartIndex = input.IndexOf('\'', startIndex, endIndex - startIndex);

            if ((doubleQuoteStartIndex == -1 || input.Count(c => c == '"') < 2) && (singleQuoteStartIndex == -1 || input.Count(c => c == '\'') < 2))
            {
                return input;
            }


            char quoteChar;
            //single quote
            if (doubleQuoteStartIndex == -1 || singleQuoteStartIndex < doubleQuoteStartIndex)
            {
                startIndex = singleQuoteStartIndex;
                quoteChar  = '\'';
            }
            //double quote
            else
            {
                startIndex = doubleQuoteStartIndex;
                quoteChar  = '"';
            }

            endIndex = input.LastIndexOf(quoteChar, startIndex, endIndex - startIndex);


            return input.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        ///     Finds the index of the closing bracket that matches a specified opening bracket within the input string.
        ///     This method searches forward from the position immediately after the given <paramref name="bracketIndex" />,
        ///     and accounts for nested brackets of the same type. If a matching closing bracket is found, its index is returned.
        ///     If no closing bracket is found, or the input is invalid, the method returns -1.
        ///     \ is an escape character.
        /// </summary>
        /// <param name="input">
        ///     The input string in which to search for the matching closing bracket.
        /// </param>
        /// <param name="bracketIndex">
        ///     The index of the known opening bracket character within <paramref name="input" /> where the search should begin.
        /// </param>
        /// <param name="openingBracket">
        ///     The character representing the opening bracket (e.g., '(' or '{').
        /// </param>
        /// <param name="closingBracket">
        ///     The character representing the closing bracket (e.g., ')' or '}').
        /// </param>
        /// <returns>
        ///     The index of the corresponding closing bracket if found; otherwise, -1 if no match is found or if input is invalid.
        /// </returns>
        public static int FindClosingBracket(this string input, int bracketIndex, char openingBracket, char closingBracket)
        {
            int depth = 1;
            if (bracketIndex == -1 || (bracketIndex + 1) >= input.Length)
            {
                return -1;
            }

            for (int i = bracketIndex + 1; i < input.Length; i++)
            {
                char c = input[i];

                if (c == '\\')
                {
                    i++;
                    continue;
                }
                
                if (c == openingBracket)
                {
                    depth++;
                }
                else if (c == closingBracket)
                {
                    depth--;
                }

                if (depth == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public static string Escape(this string input, params char[] escapeChars)
        {
            if (string.IsNullOrEmpty(input) || escapeChars == null || escapeChars.Length == 0)
            {
                return input;
            }

            // Build a character class pattern with each char individually escaped
            string pattern = "[" + string.Join("", escapeChars.Select(c => "\\" + c)) + "]";
            
            return Regex.Replace(input, pattern, @"\$&");
        }

        public static string Unescape(this string input, params char[] escapeChars)
        {
            if (string.IsNullOrEmpty(input) || escapeChars == null || escapeChars.Length == 0)
            {
                return input;
            }

            // Build a pattern that matches backslash followed by any of the escape chars
            string pattern = @"\\" + "[" + string.Join("", escapeChars.Select(c => "\\" + c)) + "]";
            
            return Regex.Replace(input, pattern, m => m.Value.Substring(1));
        }

        public static bool ContainsUnEscaped(this string input, char chr)
        {
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (c == '\\')
                {
                    i++;
                    continue;
                }
                
                if (c == chr)
                {
                    return true;
                }
            }
            return false;
        }
    }
}