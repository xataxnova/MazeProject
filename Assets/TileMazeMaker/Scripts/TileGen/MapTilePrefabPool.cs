using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen
{
    public interface iResourceProvider
    {
        Object LoadObject(string path_name);
    }

    /// <summary>
    /// 方便以后扩展，比如需要用到AssetBundle的时候
    /// </summary>
    public class DefaultResourceLoader : iResourceProvider
    {
        public Object LoadObject(string path_name)
        {
            return Resources.Load(path_name);
        }
    }

    public static class MapTilePrefabPool
    {
        private static int s_MaxPoolSize;
        private static int s_PerPrefabPoolSize;
        private static Vector3 FarPoint = new Vector3(0,999999,0);

        private static iResourceProvider m_ResourceLoader;
        public static void ResetPrefabPool( iResourceProvider resource_provider = null,int max_pool_size = 400, int per_prefab_poo_size = 40 )
        {   //暂时不清空资源！！！
            if (resource_provider == null)
            {
                m_ResourceLoader = new DefaultResourceLoader();                
            }
            else
            {
                m_ResourceLoader = resource_provider;
            }
        }

        //object pool: avoid extra Resourses.Load() cost extra io.
        private static Dictionary<string, Object> m_ObjectPool = new Dictionary<string, Object>();
        //prefab pool: 
        private static Dictionary<string, List<GameObject>> m_PrefabPool = new Dictionary<string, List<GameObject>>();

        /// <summary>
        /// 用名字来建立Prefab pool 的索引
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static GameObject GetTileObject( TilePrefabConfig config )
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("{0}/{1}", config.prefab_path, config.group_name);            
            Object obj = GetObject(sb.ToString());
            GameObject new_game_object = null;

            if (obj != null)
            {
                new_game_object = GameObject.Instantiate(obj as GameObject);
                new_game_object.name = config.group_name;
            }
            return new_game_object;
        }

        /// <summary>
        /// 在指定位置，读取资源，并且强制缓存Object对象，这个不允许不缓存。
        /// </summary>
        /// <param name="path_name"></param>
        /// <returns></returns>
        private static Object GetObject(string path_name)
        {
            Object obj = null;
            m_ObjectPool.TryGetValue(path_name, out obj);
            if (obj == null)
            {
                obj = m_ResourceLoader.LoadObject(path_name);
                if (obj != null)
                {
                    m_ObjectPool.Add(path_name, obj);
                }
                else
                {
                    Debug.LogError("Error prefab not found at " + path_name);
                }
            }

            return obj;
        }
    }
}