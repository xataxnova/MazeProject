using UnityEngine;
using System.Collections;

namespace TileMazeMaker 
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance;
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                OnAwake();
            }
            else
            {
                Destroy(gameObject);
            }            
        }

        virtual public void OnAwake() { }
    }
}

