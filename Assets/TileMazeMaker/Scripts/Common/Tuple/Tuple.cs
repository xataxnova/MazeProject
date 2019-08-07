using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tuple
{
    public static class Tuple
    {
        public static TwoTuple<A,B> tuple<A, B>(A a, B b )
        {
            return new TwoTuple<A, B>(a, b);
        }

        public static TwoTupleList<A,B> tupleList<A, B>()
        {
            return new TwoTupleList<A, B>();
        }

        public static ThreeTuple<A, B, C> tuple<A, B, C>(A a, B b, C c)
        {
            return new ThreeTuple<A, B, C>(a,b,c);
        }

        public static ThreeTupleList<A, B, C> tupleList<A, B, C>()
        {
            return new ThreeTupleList<A, B, C>();
        }

        public static FourTuple<A, B, C, D> tuple<A, B, C, D>(A a, B b, C c, D d)
        {
            return new FourTuple<A, B, C, D>(a, b, c, d);
        }

        public static FourTupleList<A, B, C, D> tupleList<A, B, C, D>()
        {
            return new FourTupleList<A, B, C, D>(); 
        }

    }

}
