using System;
using UnityEngine;

namespace VNTags
{
    /// <summary>
    ///     Music struct containing the Name/ID, audio asset and parameters to play it,
    ///     music is typically looped, VNSound is typically player once
    ///     Fields are public for serialization purposes,
    ///     please use the functions to get the data rather than directly accessing the fields
    /// </summary>
    [Serializable]
    public class VNMusicData : IVNData
    {
        [Tooltip("ID for the music, case insensitive")]
        [SerializeField] private string name;

        [SerializeField] private string[] alias;

        [SerializeField] private AudioClip soundAsset;

        public string Name
        {
            get { return name; }
        }

        public string[] Alias
        {
            get { return alias; }
        }
    }
}