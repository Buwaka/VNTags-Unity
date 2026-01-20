using System;
using UnityEngine;

namespace CharlieMadeAThing.NeatoTags.Core
{
    //[CreateAssetMenu( fileName = "New NeatoTag", menuName = "Neato Tags/New Tag", order = 0 )]
    [Serializable]
    public class NeatoTag : ScriptableObject
    {
        [SerializeField] private Color  color   = Color.gray;
        [SerializeField] private string comment = string.Empty;


        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
            }
        }
    }
}