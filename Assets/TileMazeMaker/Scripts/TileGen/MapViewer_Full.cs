using UnityEngine;

namespace TileMazeMaker.TileGen
{
    public class MapViewer_Full : MonoBehaviour,IMapViewer
    {
        Transform m_MapRoot;
        MapArchiveFile m_ArchiveFile;
        public void InitMapViewer(MapArchiveFile file,Transform root)
        {
            m_ArchiveFile = file;
            m_MapRoot = root;
            ShowMapAt(0, 0);
        }

        public void ShowMapAt(int center_x, int center_y)
        {
            TileMapBaseConfig config = m_ArchiveFile.GetConfigFile<TileMapBaseConfig>();
            if (config != null)
            {
                TilePrefabConfig tpc = null;

                for (int y = 0; y < config.height; y++)
                {
                    for (int x = 0; x < config.width; x++)
                    {
                        //这个不要做检查，浪费时间
                        tpc = config.GetTilePrefabConfig(m_ArchiveFile[x * config.width + y]);
                        if (tpc != null)
                        {
                            tpc.CreateInstance(x, y, config.grid_size, m_MapRoot);
                        }
                    }
                }
            }
        }
    }
}

