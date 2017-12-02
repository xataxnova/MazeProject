using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm.Maze 
{
    public class MazeAlgorithm_Wilson : MazeAlgorithm
    {
        //1-寻找任意点A，作为目标点，标记为Visited。
        //2-寻找任意点B，作为起始点。
        //3-从B寻找A，找到非闭合路径，将路径挖通，并且标记为Visited
        //4-遇到闭合路径，从新挖。
        //5-随机找B2，重复过程
        //6-直到所有的点都被访问过
        protected override void Generate()
        {
            //注意，一个格子，要么在Unvisited 里面，要么在Visited里面
            //所以这里不需要两个数组，实际上，Visited的记录是毫无意义的。
            //更重要的是Unvisited里面还剩什么。
            //而判断 visited.Contain( x ) 的等价命题是 unvisited.Contain( x ) == false
            //这样起码可以节省一半的内存空间
            List<IMazeCell> unvisited = new List<IMazeCell>(cells);
            List<IMazeCell> temp_path = new List<IMazeCell>();

            IMazeCell first = unvisited[Random.Range(0, unvisited.Count)];
            unvisited.Remove(first);

            IMazeCell random_start = unvisited[Random.Range(0, unvisited.Count)];

            while (unvisited.Count > 0) 
            {
                temp_path.Clear();
                temp_path.Add(random_start);

                while (true) 
                {
                    //如果有强烈需求，可以考虑增加接口，暂时先这样做。
                    EMazeDirection dir = temp_path[temp_path.Count - 1].GenRandomNeighbourDirection();
                    IMazeCell next = temp_path[temp_path.Count - 1].GetNeighbour(dir);

                    //没有形成环路
                    if (unvisited.Contains(next) == false)
                    {
                        //开始联通了
                        temp_path.Add(next);

                        for (int i = 0; i < temp_path.Count - 1; i++)
                        {
                            unvisited.Remove(temp_path[i]);
                            temp_path[i].ConnectionTo(temp_path[i].LastRandomNeibourDirection);
                        }

                        if (unvisited.Count > 0)
                        {
                            random_start = unvisited[Random.Range(0, unvisited.Count)];
                        }
                        break;
                    }
                    //形成了环路
                    else if (temp_path.Contains(next) == true) 
                    {
                        break;
                    }

                    temp_path.Add(next);
                }
            }
        }
    }

}