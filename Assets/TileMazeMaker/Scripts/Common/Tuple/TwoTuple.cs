using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tuple
{
    public class TwoTuple<A, B>
    {
        public A first;
        public B second;

        public TwoTuple(A first, B second)
        {
            this.first = first;
            this.second = second;
        }           

        public override string ToString()
        {
            return string.Format("({0},{1})",first,second);
        }
    }

}
