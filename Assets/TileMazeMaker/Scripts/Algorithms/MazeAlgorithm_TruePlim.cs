using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm.Maze 
{
    public class MazeAlgorithm_TruePlim : MazeAlgorithm
    {
        protected override void Generate()
        {
            //Use GroupID as COST !!!!
            List<IMazeCell> actived = new List<IMazeCell>(cells);

            //Make sure each cell has different priority.
            while (actived.Count > 1) 
            {
                IMazeCell random = actived[Random.Range(0, actived.Count)];
                random.GroupID = actived.Count + 1;
                actived.Remove(random);
            }

            List<IMazeCell> candidate = new List<IMazeCell>( actived[0].Neighbours );
            List<EMazeDirection> candidate_connect_direction = new List<EMazeDirection>((int)EMazeDirection.DirectionCount);

            //actived now has only one element in it, active[0]. it do not need to have a priority
            while (actived.Count < cells.Count) 
            {
                IMazeCell min = candidate[0];
                for (int i = 0; i < candidate.Count; i++) 
                {
                    if (min.GroupID > candidate[i].GroupID) 
                    {
                        min = candidate[i];
                    }
                }

                candidate.Remove(min);

                candidate_connect_direction.Clear();
                for (int i = 0; i < (int)EMazeDirection.DirectionCount; i++)
                {
                    EMazeDirection dir = (EMazeDirection)i;
                    IMazeCell neighbour = min.GetNeighbour(dir);
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

                min.ConnectionTo(candidate_connect_direction[Random.Range(0, candidate_connect_direction.Count)]);
                actived.Add(min);
            }
        }
    }

}
