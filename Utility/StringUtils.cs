using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VNTags.Utility
{
    public static class StringUtils
    {
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

        public static string EncloseNextWord(this string text, string tag, string openingTag, string closingTag)
        {
            var options = RegexOptions.Compiled;

            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            // capturing the word that is connected to it
            string pattern = $"{Regex.Escape(tag)}(\\w+)";

            // $1 refers to the content of the first capturing group, which is the number.
            string replacement = $"{openingTag}$1{closingTag}";

            return Regex.Replace(text, pattern, replacement, options);
        }

        public static string SwapEnclosing(this string text, string enclosingTag, string openingTag, string closingTag)
        {
            // Fix the RegexOptions combination using the bitwise OR operator.
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Compiled;

            // Escape the tags to ensure they are treated as literal strings, not regex characters.
            string escapedTag  = Regex.Escape(enclosingTag);
            string pattern     = $"({escapedTag})(.*?)(?s)({escapedTag})";
            string replacement = $"{openingTag}$2{closingTag}";

            return Regex.Replace(text, pattern, replacement, options);
        }

        public static string Enclose(this string text, string searchWord, string openingTag, string closingTag)
        {
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Compiled;

            string pattern     = @"\b("     + Regex.Escape(searchWord) + @")\b";
            string replacement = openingTag + "$1"                     + closingTag;

            return Regex.Replace(text, pattern, replacement, options);
        }

        public static string Enclose(this string text, string searchWord, string enclosingTag)
        {
            return Enclose(text, searchWord, enclosingTag, enclosingTag);
        }

        public static string EncloseBatch(this string text,
                                          IEnumerable<(string SearchWord, string OpeningTag, string ClosingTag)>
                                              wordTags)
        {
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Compiled;

            // Create a lookup dictionary for the tags.
            var tagLookup = wordTags.ToDictionary(
                                                  term => term.SearchWord,
                                                  term => (term.OpeningTag, term.ClosingTag),
                                                  StringComparer.OrdinalIgnoreCase // Case-insensitive lookup
                                                 );

            // Build a single pattern from all search words.
            // Wrap each word in a capturing group.
            var    patterns      = tagLookup.Keys.Select(word => $"\\b({Regex.Escape(word)})\\b");
            string masterPattern = string.Join("|", patterns);

            return Regex.Replace(text,
                                 masterPattern,
                                 match =>
                                 {
                                     string matchedWord = match.Value;

                                     if (tagLookup.TryGetValue(matchedWord,
                                                               out (string OpeningTag, string ClosingTag) tags))
                                     {
                                         return $"{tags.OpeningTag}{matchedWord}{tags.ClosingTag}";
                                     }

                                     // If for some reason the word isn't in the dictionary,
                                     // return the original match to not break the string.
                                     return matchedWord;
                                 },
                                 options);
        }
    }
}