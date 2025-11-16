using System;
using System.Collections;
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

        public void AddUnique(VNTag tag, bool first = true)
        {
            foreach (var ctag in GetCollection())
            {
                if (ctag.GetType() == tag.GetType())
                {
                    return;
                }
            }

            if (first)
            {
                AddFirst(tag);
            }
            else
            {
                AddLast(tag);
            }
        }

        public void RemoveofType(VNTag tag)
        {
            RemoveofType(tag.GetType());
        }
        
        public void RemoveofType(Type type)
        {
            LinkedListNode<VNTag> currentNode = First;
            while (currentNode != null)
            {
                var next = currentNode.Next;
                if (currentNode.Value.GetType() == type)
                {
                    Remove(currentNode);
                }
                
                currentNode = next;
            }
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

        public bool ExecuteAll(VNTagContext context, int retries = 3)
        {
            if ((Count <= 0) || (First.Value == null))
            {
                return true;
            }
            
            VNTag tag = First.Value;
            context.SetMainCharacter(_currentCharacter);

            int tries = 0;
            while (tries < retries && tag != null)
            {
                tag.BaseExecute(context, out bool isFinished);

                if (isFinished)
                {
                    RemoveFirst();
                    tries = 0;
                    tag = First?.Value;
                }
                else
                {
                    tries++;
                }
            }

            return Count == 0;
        }

        public IEnumerator ExecuteAsync(VNTagContext context, Action onComplete = null)
        {
            while (Count > 0)
            {
                Tick(context);
                yield return null;
            }
            
            onComplete?.Invoke();
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