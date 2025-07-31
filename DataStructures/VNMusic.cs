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
    public class VNMusic
    {
        [Tooltip("ID for the music, case insensitive")]
        public string Name;

        public AudioClip SoundAsset;
    }
}