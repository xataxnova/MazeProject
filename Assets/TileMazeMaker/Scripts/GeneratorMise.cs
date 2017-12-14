using UnityEngine;

namespace TileMazeMaker.TileGen
{

    public class TileMapBaseConfig : ScriptableObject
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

        /// <summary>
        /// 边界检查，看坐标是否在范围之内，正常生成地图不要检查，费时间
        /// 但是ViewPort Streaming的时候必须要检查。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool CheckValidXY(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
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

    public interface IMapArchiveGenerator
    {
        void GenerateMapArchiveFile( MapArchiveFile file );
    }

    //TODO:以后改成能兼容迷宫的！！！
    public interface IMapViewer
    {
        void InitMapViewer(MapArchiveFile file,Transform map_root);
        void ShowMapAt(int x, int y);
    }
}
