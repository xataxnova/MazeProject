using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TileMazeMaker.TileGen
{

    public abstract class MapGenerator_Inspector : Editor
    {
        IMapGenerator owner;

        public override void OnInspectorGUI()
        {
            if (owner == null) 
            {
                owner = target as IMapGenerator;
            }
            ShowCommonCommands();
            base.OnInspectorGUI();
        }

        protected void ShowCommonCommands() 
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate")) 
            {
                owner.BuildMap();
            }
            if (GUILayout.Button("Clear")) 
            {
                owner.ClearMap();
            }
            /*
            if (GUILayout.Button("Save")) 
            {
                Debug.LogWarning("Not implement");
            }
            if(GUILayout.Button("Load"))
            {
                Debug.LogWarning("Not implement");
            }*/
            EditorGUILayout.EndHorizontal();
        }
    }

}

