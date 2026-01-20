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
    [CreateAssetMenu(fileName = "VNSound", menuName = "VNTags/Data/VNSound")]
    public class VNSoundData : ScriptableObject, IVNData
    {

        private static VNSoundData _none;

        [Tooltip("ID for the sound, case insensitive")] [SerializeField]
        private string name;

        [SerializeField] private string[] alias;

        [SerializeField] private AudioClip soundAsset;

        public AudioClip SoundAsset
        {
            get { return soundAsset; }
        }

        public static IVNData None
        {
            get
            {
                if (_none == null)
                {
                    _none = CreateInstance<VNSoundData>();
                    //_none.name = "None";
                }
                return _none;
            }
        }

        // probably a layer
        // default volume


        public string Name
        {
            get { return name; }
        }

        public string[] Alias
        {
            get { return alias; }
        }

        public string DataType { get; } = "Sound";
        public IVNData NoneData
        {
            get
            {
                return None;
            }
        }
    }
}