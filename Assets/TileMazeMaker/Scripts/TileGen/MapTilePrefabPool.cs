using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen
{
    public class MapTilePrefabPool:MonoBehaviour,iResourceLoader
    {
        [SerializeField]
        private int m_MaxPoolSize;
        [SerializeField]
        private int m_PerPrefabPoolSize;
        [SerializeField]
        private Transform m_FarFarAway;

        private iResourceProvider m_ResourceLoader;

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

        /// <summary>
        /// 重置资源管理器，用Init可能更合适，因为当前版本并没有真正的Release掉所有的资源。
        /// </summary>
        /// <param name="provider">如果提供了其他资源供应者，如AssetBundle版本的实现，则可以替换当前的</param>
        public void Reset(iResourceProvider provider = null)
        {
            if (provider == null)
            {
                m_ResourceLoader = new DefaultResourceLoader();
            }
            else
            {
                m_ResourceLoader = provider;
            }
        }

        //object pool: avoid extra Resourses.Load() cost extra io.
        private Dictionary<string, Object> m_ObjectPool = new Dictionary<string, Object>();
        //prefab pool: 
        private Dictionary<string, List<GameObject>> m_PrefabPool = new Dictionary<string, List<GameObject>>();
        private List<GameObject> pPrefabPoolProbe;
        
        /// <summary>
        /// 用名字来建立Prefab pool 的索引
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public GameObject GetTileObject( TilePrefabConfig config )
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("{0}/{1}", config.prefab_path, config.group_name);            
            
            GameObject new_game_object = null;

            if (m_PrefabPool.TryGetValue(config.group_name, out pPrefabPoolProbe) && 
                pPrefabPoolProbe.Count > 0)
            {
                new_game_object = pPrefabPoolProbe[pPrefabPoolProbe.Count - 1];
                pPrefabPoolProbe.RemoveAt(pPrefabPoolProbe.Count - 1);
            }
            else
            {
                Object obj = GetObject(sb.ToString());
                if (obj != null)
                {
                    new_game_object = GameObject.Instantiate(obj as GameObject);
                    new_game_object.name = config.group_name;
                }
            } 
            return new_game_object;
        }

        /// <summary>
        /// 主动释放缓存的GameObject对象
        /// </summary>
        void OnDestroy()
        {
            List<string> to_be_clean_key = new List<string>();
            foreach (var prefab_list_key in m_PrefabPool.Keys)
            {
                to_be_clean_key.Add(prefab_list_key);
            }

            List<GameObject> gobj_list = null;
            for (int i = 0; i < to_be_clean_key.Count; i++)
            {
                gobj_list = m_PrefabPool[to_be_clean_key[i]];
                for (int gobj_index = 0; gobj_index < gobj_list.Count; gobj_index++)
                {
                    if (Application.isPlaying == false)
                        GameObject.DestroyImmediate(gobj_list[gobj_index]);
                    else
                        GameObject.Destroy(gobj_list[gobj_index]);
                }

                gobj_list.Clear();
            }
            m_PrefabPool.Clear();
        }

        /// <summary>
        /// 如果一开始的操作不够流畅，那么在加载场景的时候，就要考虑预缓存一部分prefab了。
        /// 具体的算法可以读取配置文件，或者统计每种资源的场上个数来确定。
        /// 这个函数也可以在“idle”状态的时候被调用，开始自动Balancing算法来缓慢的优化pool的构成。
        /// 这是更进一步的算法，目前还用不到。
        /// 
        /// 典型的空间换时间算法，考虑到Tile的单个内存占用量比较小，所以并不会太费。
        /// 注意权衡：如果Tile是带Collider的，那么需要权衡是为Tile加上Kinematic的刚体划算（初步分析还是这个划算）
        /// ，还是直接Spawn一个新的划算。因为移动不带刚体的Collider也是非常耗的……
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="warm_up_count"></param>
        public void WarmUpFor(TilePrefabConfig config, int warm_up_count)
        {
            //注册新的
            if (m_PrefabPool.ContainsKey(config.group_name) == false)
            {
                m_PrefabPool[config.group_name] = new List<GameObject>();
            }

            for (int i = 0; i < warm_up_count; i++)
            {
                //直接生成一个并走回收流程，简单粗暴！这样一操作，对象池里就增加了一个备胎。
                RecycleGameObject(GetTileObject(config));
            }
        }

        private Transform m_TempraryTransform = null;
        public void RecycleGameObject(GameObject prefab_instance)
        {
            string key = prefab_instance.name;

            if (m_PrefabPool.ContainsKey(key) == false)
            {
                m_PrefabPool[key] = new List<GameObject>();
            }

            if (m_PrefabPool[key].Count < m_PerPrefabPoolSize)
            {
                m_TempraryTransform = prefab_instance.transform;                
                m_TempraryTransform.SetParent(m_FarFarAway);
                m_TempraryTransform.localPosition = Vector3.zero;
                m_PrefabPool[key].Add(prefab_instance);
            }
            else//destroy gameobject if pool is full
            {
                if (Application.isPlaying == false)
                {
                    DestroyImmediate(prefab_instance);
                }
                else
                {
                    DestroyObject(prefab_instance);
                }
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var key in m_PrefabPool.Keys)
            {
                sb.Append("key ");
                sb.Append(key);
                sb.Append(" size ");
                sb.Append(m_PrefabPool[key].Count);
                sb.Append(" | ");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 在指定位置，读取资源，并且强制缓存Object对象，这个不允许不缓存。
        /// </summary>
        /// <param name="path_name"></param>
        /// <returns></returns>
        private Object GetObject(string path_name)
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