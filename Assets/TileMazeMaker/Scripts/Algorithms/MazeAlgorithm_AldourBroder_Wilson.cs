using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm.Maze 
{
    public class MazeAlgorithm_AldourBroder_Wilson : MazeAlgorithm
    {
        protected override void Generate()
        {
            IMazeCell random_start = GetRandomCell();
            List<IMazeCell> unvisited = new List<IMazeCell>(cells);
            unvisited.Remove(random_start);

            //前半段，用AldourBroder
            while (unvisited.Count > cells.Count / 2) 
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
                        unvisited.Remove(random_start);
                    }
                    else
                    {
                        random_start = pending_next;
                        continue;
                    }
                }
            }

            //后半段，用Wilson
            List<IMazeCell> temp_path = new List<IMazeCell>();
            random_start = unvisited[Random.Range(0, unvisited.Count)];

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