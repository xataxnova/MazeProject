using UnityEngine;

namespace TileMazeMaker.TileGen
{
    public class MapViewer_Full : MapViewer
    {

        public override void InitMapViewer(MapArchiveFile file, Transform root)
        {
            base.InitMapViewer(file, root);
            ShowMapAt(0, 0);
        }
        
        override public void ShowMapAt(int center_x, int center_y)
        {
            TileMapBaseConfig config = m_ArchiveFile.GetConfigFile<TileMapBaseConfig>();
            if (config != null)
            {
                TilePrefabConfig tpc = null;

                for (int y = 0; y < config.height; y++)
                {
                    for (int x = 0; x < config.width; x++)
                    {
                        tpc = config.GetTilePrefabConfig(m_ArchiveFile[x * config.width + y]);
                        if (tpc != null)
                        {
                            SpawnTileMapAt(
                                        x,
                                        y,
                                        m_Config.grid_size,
                                        m_MapRoot,
                                        tpc);
                        }
                    }
                }
            }
        }
    }
}

