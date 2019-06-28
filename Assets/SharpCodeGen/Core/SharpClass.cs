using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SharpCodeGen
{
    //not supprot partial class now
    //not support generaic class now
    //support inherient
    //support interfaces
    public class SharpClass: SharpType
    {
        public ESharpAccessLevel access_level = ESharpAccessLevel.PUBLIC;
        public SharpClass parent_class;        
    }
}
