using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen
{
    public class TilePrefabConfig : ScriptableObject
    {
        public int index;
        public string prefab_path;
        public string group_name;
        public float vertical_height;        
        public bool random_direction;
        public int occurancy;
    }

}
