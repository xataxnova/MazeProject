using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen
{

    public class TileMapBaseConfig:ScriptableObject
    {
        public string map_name;
        public int width;
        public int height;
        public float grid_size;
        public TileThemeConfig theme_config;
        
        /// <summary>
        /// 通过id，可以是存储于TileMapFile的id，也可以是直接计算出来的id。
        /// 获得Config文件。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TilePrefabConfig GetTilePrefabConfig(int id)
        {
            return theme_config[id];
        }  
    }

    public interface IMapGenerator
    {
        /// <summary>
        /// Call this method to build maze at runtime or in editor.
        /// </summary>
        void BuildMap();
        void ClearMap();
    }

    public interface IMapDataGenerator 
    {
        TileMapData GenerateTileMapData();
    }
}
