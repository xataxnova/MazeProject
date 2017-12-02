using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen
{
    using TileMazeMaker;

    public class MapGenerator_Perlin : MonoBehaviour,IMapDataGenerator,IMapGenerator
    {
        [SerializeField]
        MapGenerator_Perlin_Config config;

        [SerializeField]
        Transform map_root;
        TileMapData map_data;

        const int origin_bound = 1024;

        public TileMapData GenerateTileMapData()
        {
            //这个以后考虑如何自动化进行。容易遗忘的操作！
            config.theme_config.RebuildTileThemeConfig();

            Debug.Log("GenerateTileMapData");
            PerlinRandom pr = new PerlinRandom();
            pr.width = config.width;
            pr.height = config.height;
			pr.x_org = config.random_origin_position ? Random.Range(0, origin_bound) : config.x_org;
			pr.y_org = config.random_origin_position ? Random.Range(0, origin_bound) : config.y_org;
            pr.scale = config.scale;
            pr.Generate();

            TileMapData tile_map_date = new TileMapData();

            for (int y = 0; y < config.height; y++) 
            {
                for( int x = 0; x< config.width;x++)
                {
                    tile_map_date[SharedUtil.PointHash(x, y)] = config.GetPrefabIndex(pr.GetValueAt(x, y));
                }
            }

            return tile_map_date;
        }

        public void BuildMap()
        {
            ClearMap();

            map_data = GenerateTileMapData();
            Debug.Log("Begin Instance Map BuildMap ");
            for (int y = 0; y < config.height; y++) 
            {
                for (int x = 0; x < config.width; x++) 
                {
                    int index = SharedUtil.PointHash(x, y);
                    TilePrefabConfig tpc = config.GetTilePrefabConfig(map_data[index]);
                    if (tpc != null) 
                    {
                        tpc.CreateInstance(x, y, config.grid_size, map_root);                      
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
