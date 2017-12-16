using UnityEngine;
using UnityEditor;

namespace TileMazeMaker.TileGen
{
    [CustomEditor(typeof(MapViewer_Streamer))]
    public class MapViewer_StreamerInspector : Editor
    {
        MapViewer_Streamer owner;
        public override void OnInspectorGUI()
        {
            if (owner == null)
            {
                owner = target as MapViewer_Streamer;
            }

            if (GUILayout.Button("Move To"))
            {
                owner.ShowMapAt(owner.initGazeX, owner.initGazeY);
            }

            base.OnInspectorGUI();
        }
    }
}
