using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tuple
{
    public class FourTuple<A, B, C, D> : ThreeTuple<A, B, C>
    {
        public D fourth;

        public FourTuple(A a, B b, C c, D d):base(a, b, c)
        {
            this.fourth = d;
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2},{3})", first, second, third, fourth);
        }
    }
    
}

