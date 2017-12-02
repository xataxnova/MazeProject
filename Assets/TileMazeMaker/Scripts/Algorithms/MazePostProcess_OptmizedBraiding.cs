using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm.Maze 
{
    /// <summary>
    /// Connection added only if we can find a valid inverse direction connenect to.
    /// no four slot connect together allowed!
    /// </summary>
    public class MazePostProcess_OptmizedBraiding : MazePostProcesser
    {
        static float braidingrate = 1.0f;
        public static void SetBradingRate(float val)
        {
            braidingrate = val;
        }
        
        public override void PostProcess(MazeAlgorithm maze_algorithm)
        {
            int size_x = maze_algorithm.size_x;
            int size_y = maze_algorithm.size_y;

            for (int iy = 0; iy < size_y; iy++)
            {
                for (int ix = 0; ix < size_x; ix++)
                {
                    IMazeCell cell = maze_algorithm.GetAt(ix, iy);
                    if (cell != null && cell.ConnectionCount == 1 && Random.Range(0.0f, 1.0f) < braidingrate)
                    {
                        for (int i = 0; i < (int)EMazeDirection.Invalid; i++)
                        {
                            EMazeDirection dir = (EMazeDirection)i;

                            //尚未联通的点
                            if (cell.IsConnectedTo(dir) == true) 
                            {
                                //只做反向联通，确保不会出现孤点
                                EMazeDirection inverse_dir = MazeAlgorithm.InvertDirection(dir);
                                IMazeCell invert_cell = cell.GetNeighbour(inverse_dir);
                                if (invert_cell != null) 
                                {
                                    cell.ConnectionTo(inverse_dir);
                                }
                            }//END OF IF Connected to
                        }//END OF FOR Neighbours
                    }//END OF IF Test Cell
                }
            }
        }
    }

}

