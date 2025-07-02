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
        public Text CharacterNameBox;
        public TMPro.TMP_Text TextBox;
        public GameObject DialogueWindow;
    }

    public struct VNTagLineContext
    {
        public int LineNumber;
        public string FullLine;

        public VNTagLineContext(int num, string line)
        {
            LineNumber = num;
            FullLine = line;
        }

        public override string ToString()
        {
            return LineNumber + ": " + FullLine;
        }
    }


    public interface IVNTag
    {

        void Init(string parameters, VNTagLineContext context);
        
        /// <summary>
        /// Get the tag ID to search for when parsing, case insensitive
        /// </summary>
        /// <returns></returns>
        string GetTagID();
        
        void Execute(VNTagContext context, out bool isFinished);
    }
}
