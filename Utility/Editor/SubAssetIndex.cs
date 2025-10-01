using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VNTags.Utility
{
    [Serializable]
    public class SubAssetIndex : ScriptableObject
    {
        private (string path, Object reference)[] _entries;

        public (string path, Object reference)[] Entries
        {
            get { return _entries; }
        }

        public void AddEntry(string path, Object reference)
        {
            var newArr = new List<(string path, Object reference)>();

            if ((_entries != null) && (_entries.Length > 0))
            {
                newArr.AddRange(_entries);
            }

            newArr.Add((path, reference));
            _entries = newArr.ToArray();
        }
    }
}