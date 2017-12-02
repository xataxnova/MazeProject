using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileMazeMaker;
using TileMazeMaker.Algorithm.Maze;

namespace TileMazeMaker.TileGen
{
    [System.Serializable]
    public struct MazeMaskCell
    {
        public int x;
        public int y;

        public int ToInt32()
        {
            return SharedUtil.PointHash(x, y);
        }
    }

    public class MazeGenerator_Tile_Config : TileMapBaseConfig
    {
        [Tooltip("注意不是所有的迷宫算法都支持Mask算法,后期要增加读取图片的功能")]
        [HideInInspector]//Perserved for further usage
        public List<MazeMaskCell> mask_points = new List<MazeMaskCell>();
        public EMazeAlgorithm maze_algorithm = EMazeAlgorithm.MazeAlgorithm_BinaryTree;
        [HideInInspector][Tooltip("迷宫后期处理，统计，挖洞，等等")]
        public List<EMazePostProcess> post_process = new List<EMazePostProcess>();
        public int wall_thickness = 1;
        public int road_thickness = 1;
        public string road_name = "Ground";
        public string wall_name = "Wall";

        public int tile_size_x 
        {
            get 
            {
                return GetTileSize(width);
            }
        }

        public int tile_size_y 
        {
            get 
            {
                return GetTileSize(height);
            }
        }
        
        //提取Mask数据
        public List<int> maze_mask
        {
            get
            {
                List<int> result = new List<int>();
                foreach (var cell in mask_points)
                {
                    result.Add(cell.ToInt32());
                }
                return result;
            }
        }

        //通过格子大小，计算迷宫Cell大小
        int GetTileSize( int input_size )
        {
            //int divide = input_size / (wall_thickness + road_thickness);
            //int result = divide * (wall_thickness + road_thickness) + wall_thickness;
            return input_size * (road_thickness + wall_thickness ) + wall_thickness;
        }
    }
}
