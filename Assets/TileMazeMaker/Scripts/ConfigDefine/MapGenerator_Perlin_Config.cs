using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen
{
    public class MapGenerator_Perlin_Config : TileMapBaseConfig
    {
        [System.Serializable]
        public class PerlinTheme
        {
            [Range(0, 1.0f)]
            public float low_bound = 0;
            [Range(0, 1.0f)]
            public float high_bound = 1;
            public string theme_type;
        }
                
        public float scale = 10.0f;
        public List<PerlinTheme> theme_define = new List<PerlinTheme>();
        public string def_type;
        public bool random_origin_position;
        public float x_org;
        public float y_org;
             
        
        private string GetThemeType(float p_value) 
        {
            for (int i = 0; i < theme_define.Count; i++) 
            {
                PerlinTheme theme = theme_define[i];
                if (p_value >= theme.low_bound && p_value <= theme.high_bound) 
                {
                    return theme.theme_type;
                }
            }

            return def_type;
        }

        public int GetPrefabIndex(float perlin_value) 
        {   
            string type = GetThemeType(perlin_value);
            return theme_config.GetTilePrefabConfigIndex( type );
        }
    }

}
