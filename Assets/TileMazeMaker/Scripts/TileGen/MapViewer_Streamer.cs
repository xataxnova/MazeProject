using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen
{
    public class MapViewer_Streamer : MonoBehaviour,IMapViewer
    {
        Transform m_MapRoot;
        MapArchiveFile m_ArchiveFile;
        public int initGazeX = 0;
        public int initGazeY = 0;
        public int halfViewportSize = 15;
        public int maxDestroyPerFrame = 20;
        public int maxCreatePerFrame = 10;

        Dictionary<int, int> m_ViewPortIndex = new Dictionary<int, int>();
        Dictionary<int, int> m_DyingIndex = new Dictionary<int, int>();
        Dictionary<int, int> m_SpawnIndex = new Dictionary<int, int>();
        Queue<int> m_SpawnQueue = new Queue<int>();
        Queue<int> m_DyingQueue = new Queue<int>();

        public void InitMapViewer(MapArchiveFile file, Transform map_root)
        {
            m_ArchiveFile = file;
            m_MapRoot = map_root;
            ShowMapAt(initGazeX, initGazeY);
        }

        public void ShowMapAt(int x, int y)
        {

        }
    }
}