using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker 
{
    public static class SharedUtil
    {
        //这个主要给Mask用的，Mask是不规则图形，因此不用List存，而是用Dictionary存
        //这个只是暂时的，未来的版本，将会用位图来处理。
        public static int PointHash(int x, int y) 
        {
            return x << 16 | y;
        }

        /// <summary>
        /// 这个函数需要你输入x,y坐标，以及列数目。x * pitch +y 即是列索引。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pitch"></param>
        /// <returns></returns>
        public static int PointHashToListIndex(int x, int y, int pitch)
        {
            return x * pitch + y;
        }

        #region CLASS EXTENSIONS


        public static void DestroyAllChild(this Component c)
        {
            Transform root = c.transform;

            if (root.childCount > 0)
            {
                //从后往前删除，效率高。
                for (int i = root.childCount - 1; i >= 0; i--)
                {
                    if (Application.isPlaying)
                    {
                        GameObject.Destroy(root.GetChild(i).gameObject);
                    }
                    else
                    {
                        GameObject.DestroyImmediate(root.GetChild(i).gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// 将当前Component转换成指定接口，如果当前GameOjbect上有继承了接口的组件，则返回真，否则，返回null。
        /// 可以实现这样的效果：
        /// 如果查找到某个脚本（因为他实现了某个接口），则执行某个操作，否则，执行默认行为。并且这种实现是针对接口的，非常灵活
        /// 比较耗，不要在Update中调用，比较适合在初始化的时候调用一次。
        /// 参考来源：Apex->SharedUnityExtension
        /// 
        /// 用法 this.ToInterface<IMessageHandler> 如果有某个脚本继承了这个接口，那么就会返回，否则，返回NULL。
        /// 这样就可以做到，只有当附加了继承handler的脚本，才会发送对应的消息协议，否则，不发送，或者走默认逻辑。
        /// 这样做也可以非常方便让用户自己扩展某些逻辑，比如Apex自己写了一个基础的Steering，实现了IMoveUnit接口
        /// 默认逻辑的时候，返回的 IMoveUnit接口为空，这时候使用默认的Steering Move逻辑
        /// 当你想要实现自己的Steering方法时，直接填写实现了IMoveUnit的脚本，附加到GameObeject上即可。灰常灵活。新技能Get
        public static T ToInterface<T>(this Component c, bool searchParent = false, bool required = false) where T : class
        {
            if (c.Equals(null))
            {
                return null;
            }

            return ToInterface<T>(c.gameObject, searchParent, required);
        }

        public static T ToInterface<T>(this GameObject go, bool searchParent = false, bool required = false) where T : class
        {
            if (go.Equals(null))
            {
                return null;
            }

            var c = go.GetComponent(typeof(T)) as T;

            if (c == null && searchParent && go.transform.parent != null)
            {
                return ToInterface<T>(go.transform.parent.gameObject, false, required);
            }

            if (c == null && required)
            {
                throw new MissingComponentException(string.Format("Game object {0} does not have a component of type {1}.", go.name, typeof(T).Name));
            }

            return c;
        }
        #endregion
    }     
}
