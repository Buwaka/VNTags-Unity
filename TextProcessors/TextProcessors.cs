using System;

namespace VNTags.TextProcessors
{
    public static class TextProcessors
    {
        private static string ProcessText(string text, Func<BaseTextProcessor, string, string> processorFunc)
        {
            foreach (var processor in VNTagsConfig.GetConfig().TextProcessors)
            {
                text = processorFunc(processor, text);
            }
            return text;
        }

        public static string PreProcessDialogue(string text) => ProcessText(text, (p, t) => p.PreProcessDialogue(t));
    
        public static string PostProcessDialogue(string text) => ProcessText(text, (p, t) => p.PostProcessDialogue(t));

        public static string PreProcessRawScript(string text) => ProcessText(text, (p, t) => p.PreProcessRawScript(t));

        public static string PostProcessRawScript(string text) => ProcessText(text, (p, t) => p.PostProcessRawScript(t));
    }
}