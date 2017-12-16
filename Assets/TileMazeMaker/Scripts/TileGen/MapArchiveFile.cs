using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen
{
    /// <summary>
    /// IMPORTANT!!!!!!!!!!!!!
    /// 
    /// 注意!!!!!!!!!!!!!!!!!!!!
    /// 
    /// 【1】这里本应该将生成PerlinRandomMap的算法分离出来，形成单独的类，不应该耦合在这里！!!!
    /// 
    /// 【2】存盘和读档的操作，也省略了，以后有时间慢慢填坑!!!!!
    /// 
    /// 【3】用这个类，而不是直接用一个裸数组去存，主要还是为了，封装存盘和读盘的操作。这样更灵活。这个类是可序列化的
    /// 并且以后还可以追加一些信息，比如版本信息，或者自描述信息等，或者缩略图数据等等。
    /// </summary>
    [System.Serializable]
    public class MapArchiveFile
    {
        const int C_ORITIN_BOUND = 1024;

        string m_MapConfigName;
        //存放密集数据
        int[] m_MapData;
        //存放稀疏数据，省空间。
        Dictionary<int, int> m_DecorationIndex = new Dictionary<int, int>();
        [System.NonSerialized]
        TileMapBaseConfig m_Config = null;

        //待完善，暂时考虑只为PerlinMap编写！！！
        public MapArchiveFile(TileMapBaseConfig config)
        {
            m_Config = config;
            m_MapConfigName = config.name;            
            //这样写效率高，不会频繁的Resize！！！其实这里用数组也可以...
            m_MapData = new int[config.width * config.height];
            m_DecorationIndex.Clear();
        }

        public T GetConfigFile<T>() where T : TileMapBaseConfig
        {
            return m_Config as T;
        } 

        /// <summary>
        /// 数据访问接口，直接用类名访问数组
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int this[int index]
        {
            get
            {
                return m_MapData[index];
            }
            set
            {                
                m_MapData[index] = value;
            }
        }
        
        /// <summary>
        /// 注意这里不应该这样写，算法要抽出去，不然系统耦合性会增加，扩展性会降低！！！时间紧迫，先将就了！！！！
        /// </summary>
        public void GeneratePerinMaze()
        {
            MapGenerator_Perlin_Config config = GetConfigFile<MapGenerator_Perlin_Config>();

            if (config != null)
            {
                //生成Perlin噪声地图
                PerlinRandom pr = new PerlinRandom();
                pr.width = config.width;
                pr.height = config.height;
                pr.x_org = config.random_origin_position ? Random.Range(0, C_ORITIN_BOUND) : config.x_org;
                pr.y_org = config.random_origin_position ? Random.Range(0, C_ORITIN_BOUND) : config.y_org;
                pr.scale = config.scale;
                pr.Generate();

                for (int y = 0; y < config.height; y++)
                {
                    for (int x = 0; x < config.width; x++)
                    {
                        m_MapData[x*config.width+y] = config.GetPrefabIndex(pr.GetValueAt(x, y));
                    }
                }
            }
            else
            {
                Debug.LogError("Error Map Config Not Found !");
            }
        }
    }

}
