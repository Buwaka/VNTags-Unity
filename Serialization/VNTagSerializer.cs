using System.Collections.Generic;
using System.Text;

namespace VNTags
{
    public class VNTagSerializer
    {
        public static string SerializeLine(ICollection<VNTag> tags)
        {
            var outLine = new StringBuilder("");
            var context = new VNTagSerializationContext(tags);

            CharacterTag mainCharacter = null;

            bool continueTags = false;
            var  index        = tags.GetEnumerator();
            // deal with the start tags first
            while (index.MoveNext())
            {
                VNTag tag = index.Current;

                if (tag == null)
                {
                    continue;
                }

                if (tag is CharacterTag cTag && (cTag.Character != null) && (mainCharacter == null))
                {
                    mainCharacter = cTag;
                }
                else if (tag is DialogueTag)
                {
                    continueTags = true;
                    break;
                }
                else
                {
                    outLine.Append(tag.Serialize(context));
                }
            }

            // special notation for the character
            if ((mainCharacter != null) && (mainCharacter.Character != null))
            {
                outLine.Append(mainCharacter.Character.Name + ";");
            }

            // deal with the rest of the tags
            if (continueTags)
            {
                do
                {
                    VNTag tag = index.Current;
                    if (tag == null)
                    {
                        continue;
                    }

                    outLine.Append(tag.Serialize(context));
                } while (index.MoveNext());
            }

            index.Dispose();

            return outLine.ToString();
        }
    }
}