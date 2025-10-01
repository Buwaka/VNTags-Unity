using System.Collections.Generic;
using UnityEngine;
using VNTags.Tags;

namespace VNTags
{
    public class VNTagQueue : LinkedList<VNTag>
    {
        private VNCharacterData _currentCharacter;

        public VNTagQueue()
        {
            VNTagEventAnnouncer.onCharacterTag += OnCharacterTag;
        }

        public VNTagQueue(IEnumerable<VNTag> collection) : base(collection)
        {
            VNTagEventAnnouncer.onCharacterTag += OnCharacterTag;
        }

        public int IndexOf(VNTag tag)
        {
            int i = 1;
            foreach (VNTag iTag in this)
            {
                if (iTag == tag)
                {
                    return i;
                }

                i++;
            }

            Debug.LogError("VNTagQueue: IndexOf: tag not found, (" + tag + "), returning -1");

            return -1;
        }

        /// <summary>
        ///     while it's unlikely for there to be duplicates in the list, in case there are, only the first tag will be swapped
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <returns>true if successfully overwritten, false if target couldn't be found</returns>
        public bool Overwrite(VNTag target, VNTag value)
        {
            var node = Find(target);

            if (node == null)
            {
                Debug.LogError("VNTagQueue: Overwrite: target tag not found, " + target);
                return false;
            }

            node.Value = value;
            return true;
        }


        private bool OnCharacterTag(VNTagContext context, VNCharacterData character, CharacterAction action)
        {
            if (action == CharacterAction.AddedToScene)
            {
                _currentCharacter = character;
            }

            return true;
        }

        public LinkedList<VNTag> GetCollection()
        {
            return this;
        }

        public void Tick(VNTagContext context)
        {
            if ((Count <= 0) || (First.Value == null))
            {
                return;
            }

            VNTag tag = First.Value;
            context.SetMainCharacter(_currentCharacter);

            tag.BaseExecute(context, out bool isFinished);

            if (isFinished)
            {
                RemoveFirst();
            }
        }
    }
}