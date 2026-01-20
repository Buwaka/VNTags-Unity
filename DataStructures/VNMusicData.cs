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
    [CreateAssetMenu(fileName = "VNMusic", menuName = "VNTags/Data/VNMusic")]
    public class VNMusicData : ScriptableObject, IVNData
    {

        private static VNMusicData _none;

        [Tooltip("ID for the music, case insensitive")] [SerializeField]
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
                    _none = CreateInstance<VNMusicData>();
                    //_none.name = "None";
                }
                return _none;
            }
        }

        public string Name
        {
            get { return name; }
        }

        public string[] Alias
        {
            get { return alias; }
        }

        public string DataType { get; } = "Music";
        public IVNData NoneData
        {
            get
            {
                return None;
            }
        }
    }
}