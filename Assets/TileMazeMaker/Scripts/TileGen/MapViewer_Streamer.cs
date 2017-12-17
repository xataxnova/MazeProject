using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen
{
    public class MapViewer_Streamer : MonoBehaviour, IMapViewer
    {
        Transform m_MapRoot;
        MapArchiveFile m_ArchiveFile;
        TileMapBaseConfig m_Config;
        public int initGazeX = 0;
        public int initGazeY = 0;
        public int halfViewportSize = 15;
        public int maxDestroyPerFrame = 20;
        public int maxCreatePerFrame = 10;
        public float deleteDelay = 2.0f;
        private int SpawnGeneration = 1;

        public float GridSize
        {
            get
            {
                if (m_Config != null)
                {
                    return m_Config.grid_size;
                }
                Debug.LogError("Error Config File is null");
                return -1;
            }
        }

        public int Width
        {
            get
            {
                if (m_Config != null)
                {
                    return m_Config.width;
                }
                Debug.LogError("Error Config File is null");
                return -1;
            }
        }

        public int Height
        {
            get
            {
                if (m_Config != null)
                {
                    return m_Config.height;
                }
                Debug.LogError("Error Config File is null");
                return -1;
            }
        }

        public delegate void delegateUpdateSightPosition(int x, int y);
        public delegateUpdateSightPosition onUpdateSightPosition;

        //Save the index of tile. Use member fields to avoid frequently GC.
        //key index, value tile instance id.
        Dictionary<int, GameObject> m_ActiveCells = new Dictionary<int, GameObject>();        
        //key index, value delay delete time.
        Dictionary<int, float> m_DyingCells = new Dictionary<int, float>();
        //key index,value spawn generation.
        Dictionary<int, TilePoint> m_SpawningCells = new Dictionary<int, TilePoint>();
        //key index, value tile instance id.
        Dictionary<int, int> m_PendingGenCells =new Dictionary<int, int>();
        List<int> m_KillingList = new List<int>();
        List<int> m_BirthList = new List<int>();

        private void ResetIndex()
        {
            m_ActiveCells.Clear();
            m_DyingCells.Clear();
            m_SpawningCells.Clear();
            m_PendingGenCells.Clear();
        }

        public void InitMapViewer(MapArchiveFile file, Transform map_root)
        {
            if (file != null)
            {
                ResetIndex();
                Debug.Log("Init Map Viewer Called!!!");

                m_ArchiveFile = file;
                m_MapRoot = map_root;
                m_Config = file.GetConfigFile<TileMapBaseConfig>();


                if (m_Config == null)
                {
                    Debug.LogError("Error config file is null");
                }
                TeleportTo(initGazeX, initGazeY);
            }
            else
            {
                Debug.LogError("Error archive file is null");
            }            
        }


        public void TeleportTo(int center_x, int center_y)
        {
            ShowMapAt(center_x, center_y);
            if (onUpdateSightPosition != null)
            {
                onUpdateSightPosition(center_x, center_x);
            }
        }

        public void ShowMapAt(int x, int y)
        {
            if(m_Config.CheckValidXY(x, y))
            {
                TileRect rect = DecideShowRange(x, y);
                MakeSureShowIndices(rect);

                if (Application.isPlaying == false)
                {
                    DeadOrAlive(true);
                }                
            }
            
        }

        /// <summary>
        /// 获得本次移动的位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private TileRect DecideShowRange(int x, int y)
        {
            int x1 = Mathf.Max(0, x - halfViewportSize);
            int y1 = Mathf.Max(0, y - halfViewportSize);
            int x2 = Mathf.Min(m_Config.width - 1, x + halfViewportSize);
            int y2 = Mathf.Min(m_Config.height - 1, y + halfViewportSize);

            TileRect tr = new TileRect();
            tr.SetByTwoPoint(x1, y1, x2, y2);

            return tr;
        }

        private void MakeSureShowIndices( TileRect tr )
        {
            m_PendingGenCells.Clear();
            int index_of_tile;
            int prefabid_of_tile;

            //第一步，计算玩家EndDrag的时候，需要显示的新的格子
            for (int ix = tr.x; ix <= tr.x_end; ix++)
            {
                for (int iy = tr.y; iy <= tr.y_end; iy++)
                {
                    index_of_tile = ix * m_Config.width + iy;
                    prefabid_of_tile = m_ArchiveFile[index_of_tile];

                    m_PendingGenCells.Add(index_of_tile, prefabid_of_tile);

                    //这里先添加到SpawingCells中去,真正的Spawn和Delete都在Update里面进行的。
                    //可以延迟加载和删除，防止卡顿。
                    if (m_ActiveCells.ContainsKey(index_of_tile) == false)
                    {
                        m_SpawningCells[index_of_tile] = new TilePoint(ix, iy);
                    }
                    
                    SpawnGeneration++;

                    //如果这个格子在死亡格子里面，那么，取消他的死刑。
                    if (m_DyingCells.ContainsKey(index_of_tile))
                    {
                        m_DyingCells.Remove(index_of_tile);
                    }
                }
            }

            //Debug.Log("m_spawing cells count is " + m_SpawningCells.Count);

            foreach (int key in m_ActiveCells.Keys)
            {
                //pending里面不包含这个格子，那么他是将要删掉的格子。
                if (m_PendingGenCells.ContainsKey(key) == false)
                {
                    m_DyingCells[key] = Time.timeSinceLevelLoad + deleteDelay;

                    //对于将要删除的格子，判断他是不是将要生成的格子，避免尴尬
                    if (m_SpawningCells.ContainsKey(key))
                    {
                        m_SpawningCells.Remove(key);
                    }
                }
            }

            //Debug.Log("Dying Key count " + m_DyingCells.Count);
        }

        private void DeadOrAlive( bool execute_in_editor = false )
        {
            m_KillingList.Clear();
            foreach (var key in m_DyingCells.Keys)
            {
                if (m_KillingList.Count <= maxDestroyPerFrame || execute_in_editor)
                {
                    m_KillingList.Add(key);
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < m_KillingList.Count; i++)
            {
                int index = m_KillingList[i];
                GameObject gobj = m_ActiveCells[index];
                if (gobj != null)
                {
                    //暂定，以后使用对象池，暂时避免不必要的复杂性。
                    if (Application.isPlaying)
                        Destroy(gobj);
                    else
                        DestroyImmediate(gobj);
                }
                m_ActiveCells.Remove(index);
                m_DyingCells.Remove(index);
            }

            m_BirthList.Clear();
            foreach (int key in m_SpawningCells.Keys)
            {
                if (m_BirthList.Count < maxCreatePerFrame || execute_in_editor)
                {
                    m_BirthList.Add(key);
                }
                else
                {
                    break;
                }                                
            }

            TilePrefabConfig tpc = null;

            for (int i = 0; i < m_BirthList.Count; i++)
            {
                int key = m_BirthList[i];
                tpc = m_Config.GetTilePrefabConfig(m_ArchiveFile[key]);
                if (tpc != null)
                {
                    m_ActiveCells[key] = tpc.CreateInstance(
                        m_SpawningCells[key].x,
                        m_SpawningCells[key].y, 
                        m_Config.grid_size, 
                        m_MapRoot);
                }

                m_SpawningCells.Remove(key);
            }
        }

        void Update()
        {
            DeadOrAlive();
        }


    }
}