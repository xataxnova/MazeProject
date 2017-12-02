using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm.Maze 
{
    /// <summary>
    /// 递归分割法制作迷宫。
    /// </summary>
    public class MazeAlgorithm_RecursiveDivision : MazeAlgorithm
    {
        protected override void Generate()
        {
            foreach (var cell in cells) 
            {
                if (cell.X < size_x - 1) 
                {
                    cell.ConnectionTo(EMazeDirection.East);
                }
                if (cell.Y < size_y - 1) 
                {
                    cell.ConnectionTo(EMazeDirection.North);
                }
            }

            DivideVertical(0, 0, size_x, size_y);
        }

        void DivideVertical(int start_x, int start_y, int width, int height)
        {
            int break_door_x = start_x + width / 2 - 1;
            int break_door_y = Random.Range(start_y, start_y + height);
            
            for (int i = start_y; i < start_y + height; i++)
            {
                if (i != break_door_y)
                {
                    GetAt(break_door_x, i).BuildWallTo(EMazeDirection.East);
                }
            }

            if (height >= 2)
            {
                DivideHorizontal(start_x, start_y, break_door_x - start_x + 1, height);
                DivideHorizontal(break_door_x + 1, start_y, width - break_door_x + start_x - 1, height);
            }
        }

        void DivideHorizontal(int start_x, int start_y, int width, int height)
        {
            int break_door_y = start_y + height / 2 - 1;
            int break_door_x = Random.Range(start_x, start_x + width);

            for (int i = start_x; i < start_x + width; i++)
            {
                if (i != break_door_x)
                {
                    GetAt(i, break_door_y).BuildWallTo(EMazeDirection.North);
                }
            }

            if (width >= 2)
            {
                DivideVertical(start_x, start_y, width, break_door_y - start_y + 1);
                DivideVertical(start_x, break_door_y + 1, width, height - break_door_y + start_y - 1);
            }
        }
    }

}
