using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace SharpCodeGen
{
    public class SharpCodeFile
    {
        private SharpFormator formator = new SharpFormator();
        public List<SharpUsing> block_usings = new List<SharpUsing>();

        public void ToCode(string path)
        {            
            StreamWriter sw = new StreamWriter(path);
            sw.WriteLine(this.ToString());
            sw.Close();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < block_usings.Count; i++)
            {
                sb.Append(block_usings[i].ToString());
                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}