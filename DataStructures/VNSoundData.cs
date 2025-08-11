using System;
using UnityEngine;

namespace VNTags
{
    /// <summary>
    ///     Sound Effect struct containing the Name/ID, audio asset and parameters to play it,
    ///     Fields are public for serialization purposes,
    ///     please use the functions to get the data rather than directly accessing the fields
    /// </summary>
    [Serializable]
    public class VNSoundData
    {
        [Tooltip("ID for the sound, case insensitive")]
        public string Name;

        public AudioClip SoundAsset;

        // probably a layer
        // default volume
    }
}