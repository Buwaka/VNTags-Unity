using UnityEngine;

namespace VNTags.Utility
{
    public class InfoFieldAttribute : PropertyAttribute
    {
        public string Info;
        public InfoFieldAttribute(string info)
        {
            Info = info;
        }
    }
}