using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SharpCodeGen
{
    public class SharpUsing : SharpElement
    {
        public override string ToString()
        {
            return string.Format("using {0};", this.identity_name);
        }
    }

}
