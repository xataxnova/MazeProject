using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm.Maze 
{

    public class MazeAlgorighm_HuntAndKill : MazeAlgorithm
    {
        protected override void Generate()
        {
            int visited = 1;
            Kill(visited, cells[0]);
        }

        void Kill(int visited, IMazeCell start_point ) 
        {
            if (visited <= cells.Count) 
            {
                do
                {
                    EMazeDirection move_dir = start_point.GenRandomNeighbourDirection(ESelectCondition.NoVisited);
                    
                    if (move_dir == EMazeDirection.Invalid) break;                  
                    IMazeCell next = start_point.GetNeighbour( move_dir);

                    start_point.ConnectionTo(move_dir);
                    start_point = next;
                    visited++;
                }
                while(true);

                //finish kill begin hunt.
                if (visited < cells.Count) 
                {
                    Hunt(visited);
                }
            }
        }

        void Hunt(int visited) 
        {
            IMazeCell hunt_result = null;
            
            for (int iy = size_y - 1; iy >= 0; iy--)
            //for (int iy = 0; iy < size_y;iy++ )
            {
                for (int ix = 0; ix < size_x; ix++)
                //for (int ix = size_x-1; ix >=0; ix--)
                {
                    hunt_result = GetAt(ix, iy);

                    //hunt result 必须是一个未访问过的点，并且他要链接一个已经访问过的点（隐含一定不会连接）
                    if (hunt_result.ConnectionCount == 0)
                    {
                        EMazeDirection dir = hunt_result.GenRandomNeighbourDirection(ESelectCondition.Visited);

                        if (dir != EMazeDirection.Invalid) 
                        {
                            hunt_result.ConnectionTo(dir);
                            visited++;
                            Kill(visited, hunt_result);
                            break;
                        }
                    }
                }
            }            
        }
    }

}
