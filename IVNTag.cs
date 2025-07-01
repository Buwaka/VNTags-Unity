using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace VNTags
{

    public struct VNTagContext
    {
        public Text CharacterName;
        public TMPro.TMP_Text Text;
        public GameObject DialogueBox;
    }


    public interface IVNTag
    {
        
        void Init(string parameters) {}
        
        /// <summary>
        /// Get the tag ID to search for when parsing, case insensitive
        /// </summary>
        /// <returns></returns>
        string GetTagID();
        
        void Execute(VNTagContext context, out bool isFinished);
    }
}
