using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm.Maze 
{
    public class MazeAlgorithm_Sidewinder : MazeAlgorithm
    {
        public int dig_north_rate;
        public int dig_east_rate;
        
        /// <summary>
        /// 注意：该算法对不规则图形非常不友好，如果一定要应用到不规则图形
        /// 需要做很多修改，因此建议直接禁止本算法应用到不规则图形上去。
        /// </summary>
        protected override void Generate()
        {
            List<IMazeCell> temp_cell_collection = new List<IMazeCell>();
            List<EMazeDirection> filter_dir = new List<EMazeDirection>();
            filter_dir.Add(EMazeDirection.North);
            filter_dir.Add(EMazeDirection.East);

            for (int iy = 0; iy < size_y; iy++)
            {
                for (int ix = 0; ix < size_x; ix++)
                {
                    IMazeCell cell = GetAt( ix,iy );
                    //尽管如此，还是可以利用fiter做一波优化，去掉重复代码！
                    EMazeDirection move_dir = cell.GenRandomNeighbourDirection(ESelectCondition.None, filter_dir);
                    if (move_dir == EMazeDirection.North) 
                    {
                        temp_cell_collection.Add(cell);
                        IMazeCell dig_cell = temp_cell_collection[Random.Range(0, temp_cell_collection.Count)];
                        dig_cell.ConnectionTo(EMazeDirection.North);
                        temp_cell_collection.Clear();
                    }
                    else if (move_dir == EMazeDirection.East) 
                    {
                        temp_cell_collection.Add(cell);
                        cell.ConnectionTo(EMazeDirection.East);
                    }
                }
            }
        }

        bool SelectNorth
        {
            get
            {
                return Random.Range(0, dig_east_rate + dig_north_rate) < dig_north_rate;
            }
        }
    }

}