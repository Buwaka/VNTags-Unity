namespace VNTags.TextProcessors
{
    public abstract class ITextProcessor
    {
        /// <summary>
        /// change the dialogue before it is processed,
        /// this is meant for text processors that work on the raw dialogue.
        /// there's no guarantee that your class will be first,
        /// but by differentiating pre- and post- we can alleviate order-based most issues.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual string PreProcessDialogue(string text)
        {
            return text;
        }
        
        /// <summary>
        /// change the dialogue after it is processed,
        /// mainly meant for text processors where the order does not matter.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
       public virtual string PostProcessDialogue(string text)
       {
           return text;
       }
       
        /// <summary>
        /// change the raw script before it is processed,
        /// this is meant for text processors that work on the raw md scripts.
        /// there's no guarantee that your class will be first,
        /// but by differentiating pre- and post- we can alleviate order-based most issues.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
       public virtual string PreProcessRawScript(string text)
       {
           return text;
       }
        
       /// <summary>
       /// change the raw script after it is processed,
       /// mainly meant for text processors where the order does not matter.
       /// </summary>
       /// <param name="text"></param>
       /// <returns></returns>
       public virtual string PostProcessRawScript(string text)
       {
           return text;
       }
       
       /// <summary>
       /// Change the name text before it is processed,
       /// for example in the name textbox.
       /// there's no guarantee that your class will be first,
       /// but by differentiating pre- and post- we can alleviate order-based most issues.
       /// </summary>
       /// <param name="text"></param>
       /// <returns></returns>
       public virtual string PreProcessName(string text)
       {
           return text;
       }
        
       /// <summary>
       /// Change the name text after it is processed,
       /// for example in the name textbox
       /// there's no guarantee that your class will be first,
       /// but by differentiating pre- and post- we can alleviate order-based most issues.
       /// </summary>
       /// <param name="text"></param>
       /// <returns></returns>
       public virtual string PostProcessName(string text)
       {
           return text;
       }
    }
}