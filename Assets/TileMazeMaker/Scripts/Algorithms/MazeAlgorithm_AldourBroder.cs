using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm.Maze 
{
    /// <summary>
    /// 基于随机行走的算法
    /// 先快后慢
    /// 因为越到后来，寻找一个没被访问过的点，越难。
    /// 随机行走。遇到没访问过的，联通之，遇到访问过的，走过去即可。
    /// </summary>
    public class MazeAlgorithm_AldourBroder : MazeAlgorithm
    {
        protected override void Generate()
        {
            IMazeCell random_start = GetRandomCell();
            int visited = 1;

            while (visited < cells.Count) 
            {
                EMazeDirection valid = random_start.GenRandomNeighbourDirection();
                IMazeCell pending_next = random_start.GetNeighbour(valid);

                if (pending_next != null) 
                {
                    //言外之意，没被访问过
                    //means, not visited.
                    if (pending_next.ConnectionCount == 0)
                    {
                        random_start.ConnectionTo(valid);
                        random_start = pending_next;
                        visited++;
                    }
                    else 
                    {
                        random_start = pending_next;
                        continue;
                    }
                }
            }
        }
    }

}
