using UnityEngine;
using System.Collections;

namespace TileMazeMaker.TileGen
{

    [System.Serializable]
    public struct TilePoint
    {
        public int x;
        public int y;

        public static System.Func<Vector3> alternativeToPosition = null;
        public static System.Func<Vector2> alternativeToPosition2D = null;

        /// <summary>
        /// 构造函数，通过两个代表格子坐标的点，构建一个TilePoint对象
        /// 注意，这个地图是从0，0开始到N，N的，暂时不支持-m，-m到m，m的地图。如果需要的话，以后会增加一个地图映射函数来解决这个问题。
        /// </summary>
        /// <param name="px"></param>
        /// <param name="py"></param>
        public TilePoint(int px, int py)
        {
            x = px;
            y = py;
        }

        /// <summary>
        /// 将一个TilePoint，散列成一个唯一的索引值，用于在Dictionary快速定位一个MapTile格子
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static int Hash(TilePoint point)
        {
            return SharedUtil.PointHash(point.x, point.y);
        }

        /// <summary>
        /// 给定一个索引值，反向创建一个TilePoint对象，用于反向索引。将索引转换成一对XY坐标。
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static TilePoint DeHash(int val)
        {
            TilePoint cmp;
            cmp.y = val & 0x00ff;
            cmp.x = val >> 16;

            return cmp;
        }

        /// <summary>
        /// 格式化输出坐标字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0},{1}]", x, y);
        }

        /// <summary>
        /// 操作符>重载。比较两个格子的大小，左下为小，右上为大
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator >(TilePoint p1, TilePoint p2)
        {
            if ((p1.x > p2.x && p1.y > p2.y) ||
                (p1.x == p2.x && p1.y > p2.y) ||
                (p1.y == p2.y && p1.x > p2.x))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 操作符>=重载。比较两个格子的大小，左下为小，右上为大
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator >=(TilePoint p1, TilePoint p2)
        {
            if ((p1.x >= p2.x && p1.y >= p2.y) ||
                (p1.x == p2.x && p1.y >= p2.y) ||
                (p1.y == p2.y && p1.x >= p2.x))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 比较两个格子的大小，左下为小，右上为大
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator <(TilePoint p1, TilePoint p2)
        {
            if ((p1.x < p2.x && p1.y < p2.y) ||
                (p1.x == p2.x && p1.y < p2.y) ||
                (p1.y == p2.y && p1.x < p2.x))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 比较两个格子的大小，左下为小，右上为大
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator <=(TilePoint p1, TilePoint p2)
        {
            if ((p1.x <= p2.x && p1.y <= p2.y) ||
                (p1.x == p2.x && p1.y <= p2.y) ||
                (p1.y == p2.y && p1.x <= p2.x))
            {
                return true;
            }

            return false;
        }

        //返回两点之间的笛卡尔距离。
        public static int DescartesDistance(TilePoint p1, TilePoint p2)
        {
            return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y);
        }

        /// <summary>
        /// 重载减法运算符，相当于计算笛卡尔距离。
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static int operator -(TilePoint p1, TilePoint p2)
        {
            return DescartesDistance(p1, p2);
        }
    }
}