using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VNTags.Utility
{
    [Serializable]
    public class SubAssetIndex : ScriptableObject
    {
        public (string path, Object reference)[] Entries { get; private set; }

        public void AddEntry(string path, Object reference)
        {
            var newArr = new List<(string path, Object reference)>();

            if (Entries != null && Entries.Length > 0)
            {
                newArr.AddRange(Entries);
            }

            newArr.Add((path, reference));
            Entries = newArr.ToArray();
        }
    }
}