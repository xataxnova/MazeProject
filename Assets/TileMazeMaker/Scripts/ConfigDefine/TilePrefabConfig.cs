using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen
{
    public class TilePrefabConfig : ScriptableObject
    {
        public int index;
        public string group_name;
        public GameObject prefab;
        public float vertical_height;        
        public bool random_direction;
        public int occurancy;



        static Vector3[] orientations = { new Vector3(0, 0, 0), new Vector3(0, 90, 0), new Vector3(0, 180, 0), new Vector3(0, 270, 0) };
        static Vector3 random_orienation
        {
            get
            {
                return orientations[Random.Range(0, orientations.Length)];
            }            
        }

        public GameObject CreateInstance(int x, int y,float grid_size, Transform parent, float override_height = -99999999) 
        {
            if (prefab != null) 
            {
                GameObject result = GameObject.Instantiate( prefab );
                result.transform.SetParent(parent);
                result.transform.localPosition = new Vector3(x * grid_size, (override_height > -99999999 ? override_height : vertical_height), y * grid_size);
                if (random_direction)
                    result.transform.localEulerAngles = random_orienation;
                return result;
            }
            return null;
        }

    }

}
