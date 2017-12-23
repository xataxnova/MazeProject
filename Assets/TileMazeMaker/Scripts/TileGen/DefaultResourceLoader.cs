using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen
{
    /// <summary>
    /// 默认的资源读取类
    /// 只提供最基本的资源读取方法
    /// 当你挂上MapTilePrefabTile脚本之后，就自动替换掉这个类了。
    /// </summary>
    public class DefaultResourceLoader:iResourceLoader
    {
        public void Reset(iResourceProvider provider = null)
        {
            Debug.LogWarning("Select DefaultResourceLoader! are your sure you want to use it????????");
        }

        public GameObject GetTileObject(TilePrefabConfig config)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("{0}/{1}", config.prefab_path, config.group_name);
            Object obj = Resources.Load(sb.ToString());
            GameObject gobj = GameObject.Instantiate(obj as GameObject);
            return gobj;
        }

        public void RecycleGameObject(GameObject prefab_instance)
        {
            if (Application.isPlaying)
            {
                GameObject.Destroy(prefab_instance);
            }
            else
            {
                GameObject.DestroyImmediate(prefab_instance);
            }
        }
    }
}