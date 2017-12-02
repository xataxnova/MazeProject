using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen
{
    /// <summary>
    /// 重构以后，这个既可以用来作为存放中间数据的媒介。
    /// 又可以用来存档。
    /// 以后会增加其他附加信息，所以单独提取出来一个Class。不光是因为为了方便序列化
    /// </summary>
    [System.Serializable]
    public class TileMapData
    {  
        //记录配置文件的路径，和name合起来，可以定位一个文件。
        public string config_file_path;
        //需要知道读取哪个ConfigFile用于重现地图。
        //TODO:以后需要知道读取哪个地图。
        //注意，这要求既要根据引用获得Config文件，又能通过名字查找Prefab文件。
        //数据+Config文件 = 重现地图。缺一不可。
        public string config_file_name;
        Dictionary<int, int> map_index = new Dictionary<int, int>();

        //先不实现，调用抛出异常
        public void Save(string save_file_path, string config_file_path) 
        {
            throw new System.NotImplementedException();
        }

        //先不实现，调用抛出异常
        public static TileMapData Load(string save_file_path)
        {
            throw new System.NotImplementedException();
        }

        public int this[int key] 
        {
            get 
            {
                return map_index[key];
            }
            set 
            {
                map_index[key] = value;
            }
        }
                
        ////高16位，右移16位即可
        //public int GetTileGroupID(int x, int y) 
        //{
        //    int key = TilePoint.Hash(x, y);
        //    int val = map_index[key];
        //    return val >> 16;
        //}

        ////低16位,抹掉高16位即可
        //public int GetTilePrefabID(int x, int y) 
        //{
        //    int key = TilePoint.Hash(x, y);
        //    int val = map_index[key];
        //    return val & 0x00ff;
        //}

        //public void SetTileGroupID(int x, int y, int value ) 
        //{
        //    int key = TilePoint.Hash(x, y);
        //    int val = map_index[key];
        //    int low = val & 0x00ff;                 //洗掉高位,记录低位
        //    map_index[key] = ( value << 16) | low;  //数据左移，写入高位与低位
        //}

        //public void SetPrefabID(int x, int y, int prefab_id) 
        //{
        //    int key = TilePoint.Hash(x, y);
        //    int val = map_index[key];
        //    val = val & 0xff00;                 //洗掉低位。
        //    map_index[key] = val | prefab_id;   //写入低位。
        //}
        
    }

}
