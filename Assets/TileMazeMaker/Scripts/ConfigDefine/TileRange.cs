using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen 
{
    public class TileRange : ScriptableObject
    {
        public int min_radius = 1;
        public int max_radius = 2;
        public int pitch = 0;
        public int center_x;
        public int center_y;
        public int version = 1;
        [SerializeField]
        public List<bool> customize_range;
    }

}
