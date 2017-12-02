using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TileMazeMaker 
{
    public abstract class Commands
    {
        virtual public void Execute(params object[] values) { }

        public static void Execute<T>(params object[] values) where T : Commands
        {
            T cmd = System.Activator.CreateInstance<T>();
            cmd.Execute(values);
        }

        //用于那些对于效率要求极高的地方，把Command缓存起来！
        public static Commands CreateCommand<T>() where T : Commands
        {
            return System.Activator.CreateInstance<T>();
        }
    }
}



