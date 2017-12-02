using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm.Maze 
{
    /// <summary>
    /// Random Plim
    /// </summary>
    public class MazeAlgorithm_SimplePlim : MazeAlgorithm
    {
        protected override void Generate()
        {
            List<IMazeCell> actived = new List<IMazeCell>(cells.Count);
            IMazeCell first = GetRandomCell();
            actived.Add(first);
            
            //加入First 的有效Neighbour作为第一波的Candidate
            List<IMazeCell> candidate = new List<IMazeCell>(first.Neighbours);            
            List<EMazeDirection> candidate_connect_direction = new List<EMazeDirection>((int)EMazeDirection.DirectionCount);

            //只要所有的格子没有被全部访问。
            while (actived.Count < cells.Count) 
            {
                IMazeCell random = candidate[Random.Range(0, candidate.Count)];
                candidate.Remove(random);

                candidate_connect_direction.Clear();
                for (int i = 0; i < (int)EMazeDirection.DirectionCount; i++) 
                {
                    EMazeDirection dir = (EMazeDirection)i;
                    IMazeCell neighbour = random.GetNeighbour(dir);
                    if (neighbour != null) 
                    {
                        if (actived.Contains(neighbour) == true)
                        {
                            candidate_connect_direction.Add(dir);
                        }
                        else 
                        {
                            if (candidate.Contains(neighbour) == false) 
                            {
                                candidate.Add(neighbour);
                            }                            
                        }                        
                    }
                }

                random.ConnectionTo(candidate_connect_direction[Random.Range(0, candidate_connect_direction.Count)]);
                actived.Add( random );
            }
        }
    }

}
