using System.Collections.Generic;
using System.Text;

namespace VNTags
{
    public class VNTagSerializer
    {


        public static string SerializeLine(ICollection<IVNTag> tags)
        {
            StringBuilder outLine = new StringBuilder("");
            
            CharacterTag mainCharacter = null;

            bool continueTags = false;
            var index = tags.GetEnumerator();
            // deal with the start tags first
            while (index.MoveNext())
            {
                IVNTag tag = index.Current;
                if (tag is CharacterTag cTag && cTag.Character != null && mainCharacter == null)
                {
                    mainCharacter = cTag;
                }
                else if (tag is DialogueTag)
                {
                    continueTags = true;
                    break;
                }
                else if (tag != null)
                {
                    outLine.Append(tag.Serialize());
                }
            }

            // special notation for the character
            if (mainCharacter != null && mainCharacter.Character != null)
            {
                outLine.Append(mainCharacter.Character.Name + ";");
            }
            
            // deal with the rest of the tags
            if (continueTags)
            {
                do
                {
                    IVNTag tag = index.Current;
                    outLine.Append(tag.Serialize());
                }while (index.MoveNext());
            }
            
            index.Dispose();

            return outLine.ToString();
        }
        
    }
}