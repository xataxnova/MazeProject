using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen
{
    using TileMazeMaker;

    public class MapGenerator_Perlin : MonoBehaviour, IMapGenerator
    {
        [SerializeField] MapGenerator_Perlin_Config config;
        [SerializeField] Transform m_MapRoot;
        [SerializeField] string m_MapName;
        MapArchiveFile m_ArchiveFile = null;
        IMapViewer m_MapViewer;

        //BuildMap一般只在Editor中运行
        public void BuildMap()
        {
            ClearMap();            
            config.theme_config.RebuildTileThemeConfig();
            m_ArchiveFile = new MapArchiveFile(config);
            m_ArchiveFile.GeneratePerinMaze();
            
            m_MapViewer = GetComponent<IMapViewer>();
            m_MapViewer.InitMapViewer(m_ArchiveFile, m_MapRoot);
        }

        public void ClearMap() 
        {
            m_MapRoot.DestroyAllChild();
        }
    }
}
