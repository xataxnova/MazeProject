using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen
{
    public abstract class MapViewer : MonoBehaviour
    {
        protected Transform m_MapRoot;
        protected MapArchiveFile m_ArchiveFile;
        protected TileMapBaseConfig m_Config;
        protected iResourceLoader m_ResourceLoader;

        virtual public void InitMapViewer(MapArchiveFile file, Transform root)
        {
            //如果没有挂在高级资源读取器或对象池化资源管理器，则走默认逻辑。使用最笨的FallBack资源管理器。
            m_ResourceLoader = GetComponent<iResourceLoader>();
            if (m_ResourceLoader == null)
                m_ResourceLoader = new DefaultResourceLoader();

            m_ResourceLoader.Reset();

            m_ArchiveFile = file;
            m_MapRoot = root;
            if (m_ArchiveFile != null)
            {
                m_Config = m_ArchiveFile.GetConfigFile<TileMapBaseConfig>();
            }            
        }
        abstract public void ShowMapAt(int center_x, int center_y);

        static Vector3[] orientations = { new Vector3(0, 0, 0), new Vector3(0, 90, 0), new Vector3(0, 180, 0), new Vector3(0, 270, 0) };
        static Vector3 random_orienation
        {
            get
            {
                return orientations[Random.Range(0, orientations.Length)];
            }
        }

        public GameObject SpawnTileMapAt(int x, int y, float grid_size, Transform parent, TilePrefabConfig prefab_config, float override_height = -99999999)
        {
            GameObject gobj = m_ResourceLoader.GetTileObject( prefab_config );
            if (gobj != null)
            {
                Transform trans = gobj.transform;
                trans.SetParent(parent);
                trans.localPosition = new Vector3(x * grid_size, (override_height > -99999999 ? override_height : prefab_config.vertical_height), y * grid_size);
                if (prefab_config.random_direction)
                {
                    trans.localEulerAngles = random_orienation;
                }
                else
                {
                    trans.localEulerAngles = Vector3.zero;
                }
            }
            return gobj;
        }
    }
}
