using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm.Maze 
{
    public class MazeAlgorithm_GrowingTree : MazeAlgorithm
    {
        public enum EGrowingTreeShampleType
        {
            Rand,
            Last,
            Rand_Last,
        }
                
        virtual protected EGrowingTreeShampleType shample_type 
        {
            get 
            {
                return EGrowingTreeShampleType.Rand_Last;
            }
        }

        protected override void Generate()
        {
            List<IMazeCell> actived = new List<IMazeCell>();
            actived.Add(GetRandomCell());

            List<EMazeDirection> candidate_connect_direction = new List<EMazeDirection>((int)EMazeDirection.Invalid);
            
            while (actived.Count > 0) 
            {
                 IMazeCell random = null;
                                
                switch (shample_type) 
                {
                    case EGrowingTreeShampleType.Rand:
                         random = actived[Random.Range(0, actived.Count)];
                        break;
                    case EGrowingTreeShampleType.Last:
                        random = actived[actived.Count - 1];
                        break;
                    case EGrowingTreeShampleType.Rand_Last:
                        random = Random.Range(0,2) == 0?  actived[Random.Range(0, actived.Count)]: actived[actived.Count - 1];
                        break;
                }

                candidate_connect_direction.Clear();
                for (int i = 0; i < (int)EMazeDirection.Invalid; i++)
                {
                    EMazeDirection dir = (EMazeDirection)i;
                    IMazeCell neighbour = random.GetNeighbour(dir);
                    if ( neighbour != null && neighbour.ConnectionCount == 0 ) 
                    {
                        candidate_connect_direction.Add(dir);
                    }
                }

                if (candidate_connect_direction.Count > 0)
                {
                    EMazeDirection select_dir = candidate_connect_direction[Random.Range(0, candidate_connect_direction.Count)];
                    actived.Add(random.GetNeighbour(select_dir));
                    random.ConnectionTo(select_dir);
                }
                else 
                {
                    actived.Remove(random);
                }
            }

        }
    }

}

