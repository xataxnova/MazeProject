using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SharpCodeGen
{
    public class SharpNamespace :SharpElement
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("namespace ");
            sb.Append(this.identity_name);
            sb.Append("\n");
            sb.Append("{");

            return sb.ToString();
        }
    }

}
