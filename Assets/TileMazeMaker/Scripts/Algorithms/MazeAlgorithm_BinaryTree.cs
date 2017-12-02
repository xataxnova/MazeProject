using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm.Maze 
{
    public class MazeAlgorithm_BinaryTree : MazeAlgorithm
    {
        /// <summary>
        /// 这个算法同样不能胜任，不规则格子的迷宫。
        /// 因为他会产生孤立点。
        /// </summary>
        protected override void Generate()
        {
            List<EMazeDirection> dir_filter = new List<EMazeDirection>();
            dir_filter.Add( EMazeDirection.North );
            dir_filter.Add( EMazeDirection.East );

            for (int iy = 0; iy < size_y; iy++)
            {
                for (int ix = 0; ix < size_x; ix++)
                {
                    IMazeCell cell = GetAt(ix, iy);

                    //算法优化，不能每次都判断是不是在墙角，那种判断对于不规则迷宫是无效的，且麻烦的。
                    //如果只希望沿着某两个方向扩展，更科学的方式是使用DirectionFilter 这样可以方便的产生更多的选择组合。
                    EMazeDirection move_to_dir = cell.GenRandomNeighbourDirection(ESelectCondition.None, dir_filter);

                    if (move_to_dir != EMazeDirection.Invalid) 
                    {
                        cell.ConnectionTo(move_to_dir);
                    }
                }
            }
        }
    }

}
