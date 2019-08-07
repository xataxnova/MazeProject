using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tuple
{
    public class ThreeTuple <A,B,C>: TwoTuple<A,B>
    {
        public C third;
        public ThreeTuple(A a, B b, C c):base(a,b)
        {
            this.third = c;
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", first,second,third);
        }

    }

}
