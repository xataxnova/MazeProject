using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileMazeMaker.Algorithm.Maze;

namespace TileMazeMaker.TileGen
{

    public static class TileRectMazeExt 
    {
        /// <summary>
        /// 方便的函数，直接遍历Rect内的所有值，并设置其Type。
        /// </summary>
        /// <param name="save_data"></param>
        /// <param name="config"></param>
        /// <param name="target_type"></param>
        public static void SetMapTileType(this TileRect rect, TileMapData save_data, TileThemeConfig theme_config, string type)
        {
            for (int iy = rect.y; iy <= rect.y_end; iy++)
            {
                for (int ix =  rect.x; ix <=  rect.x_end; ix++)
                {
                    save_data[SharedUtil.PointHash(ix, iy)] = theme_config.GetTilePrefabConfigIndex(type);
                }
            }
        }
    }

    public class MazeGenerator_Tile : MonoBehaviour,IMapDataGenerator,IMapGenerator
    {
        [SerializeField]
        MazeGenerator_Tile_Config config;
        [SerializeField]
        Transform map_root;
        TileMapData map_data;

        //挖墙算法
        protected void DigNorth(TileMapData data, TileRect rect )
        {
            TileRect dig_rect = new TileRect(rect.x, rect.y + config.road_thickness, config.road_thickness, config.wall_thickness);
            dig_rect.SetMapTileType(data, config.theme_config, config.road_name);
        }

        protected void DigSouth(TileMapData data, TileRect rect)
        {
            TileRect dig_rect = new TileRect(rect.x, rect.y - config.wall_thickness, config.road_thickness, config.wall_thickness);
            dig_rect.SetMapTileType(data, config.theme_config, config.road_name);
        }

        protected void DigEast(TileMapData data, TileRect rect)
        {
            TileRect dig_rect = new TileRect(rect.x + config.road_thickness, rect.y, config.wall_thickness, config.road_thickness);
            dig_rect.SetMapTileType(data, config.theme_config, config.road_name);
        }

        protected void DigWest(TileMapData data, TileRect rect)
        {
            TileRect dig_rect = new TileRect(rect.x - config.wall_thickness, rect.y, config.wall_thickness, config.road_thickness);
            dig_rect.SetMapTileType(data, config.theme_config, config.road_name);
        }

        List<System.Action<TileMapData, TileRect>> dig_list = new List<System.Action<TileMapData,TileRect>>();


        /// <summary>
        /// 不是特别完美，和map_data产生了耦合，不过只把他当成内部类，也无妨，只不过，其他类调用的时候，返回的是这个类的成员变量而不是临时变量！
        /// </summary>
        /// <returns></returns>
        public TileMapData GenerateTileMapData()
        {
            //重置
            TileMapData temp_data = new TileMapData();

            config.theme_config.RebuildTileThemeConfig();
            int tile_size_x = config.tile_size_x;
            int tile_size_y = config.tile_size_y;
            int total_thickness = config.wall_thickness + config.road_thickness;

            //初始化地图
            for (int iy = 0; iy < tile_size_y; iy++)
            {
                for (int ix = 0; ix < tile_size_x; ix++)
                {
                    int key = SharedUtil.PointHash(ix,iy);
                    temp_data[key] = config.theme_config.GetTilePrefabConfigIndex(config.road_name);
                }
            }

            MazeAlgorithm algorithm = 
                (MazeAlgorithm)System.Activator.CreateInstance(System.Type.GetType("TileMazeMaker.Algorithm.Maze." + config.maze_algorithm.ToString()));

            //处理Mask
            algorithm.SetMazeMaskCells(config.maze_mask);

            //设置后期处理
            for (int i = 0; i < config.post_process.Count; i++)
            {
                if (config.post_process[i] != EMazePostProcess.None)
                    algorithm.AddPostProcesser(config.post_process[i].ToString());
            }

            algorithm.BuildMaze<MazeCell>(config.width, config.height);

            dig_list.Add(DigNorth);
            dig_list.Add(DigWest);
            dig_list.Add(DigSouth);
            dig_list.Add(DigEast);

            for (int iy = 0; iy < config.height; iy++)
            {
                for (int ix = 0; ix < config.width; ix++)
                {
                    IMazeCell cell = algorithm.GetAt(ix, iy);
                    if (cell != null)
                    {
                        TileRect out_rect = new TileRect(
                            ix * total_thickness,
                            iy * total_thickness,
                            config.road_thickness + 2 * config.wall_thickness,
                            config.road_thickness + 2 * config.wall_thickness);

                        out_rect.SetMapTileType(temp_data, config.theme_config, config.wall_name);

                        TileRect rect = new TileRect(
                            ix * total_thickness + config.wall_thickness,
                            iy * total_thickness + config.wall_thickness,
                            config.road_thickness,
                            config.road_thickness);

                        rect.SetMapTileType(temp_data, config.theme_config, config.road_name);

                        for (int i = 0; i < (int)EMazeDirection.DirectionCount; i++)
                        {
                            if (cell.IsConnectedTo((EMazeDirection)i))
                            {
                                dig_list[i](temp_data, rect);
                            }
                        }
                    }
                }
            }

            return temp_data;
        }


        public void BuildMap()
        {
            ClearMap();
            map_data = GenerateTileMapData();

            for (int y = 0; y <config.tile_size_x; y++)
            {
                for (int x = 0; x < config.tile_size_y; x++)
                {
                    int index = SharedUtil.PointHash(x, y);
                    TilePrefabConfig tpc = config.GetTilePrefabConfig(map_data[index]);
                    if (tpc != null)
                    {
                        MapViewer.SpawnTileMapAt(x, y, config.grid_size, map_root, tpc);
                    }
                }
            }
        }

        public void ClearMap()
        {
            map_root.DestroyAllChild();
        }
    }

}

