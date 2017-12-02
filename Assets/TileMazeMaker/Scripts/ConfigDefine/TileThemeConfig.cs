using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.TileGen
{
    public class TileThemeConfig : ScriptableObject
    {
        public string theme_desc;

        [SerializeField]
        List<TilePrefabConfig> prefabs = new List<TilePrefabConfig>();

        Dictionary<string, List<TilePrefabConfig>> prefab_index = new Dictionary<string, List<TilePrefabConfig>>();

        public TilePrefabConfig this[int index]
        {
            get 
            {
                if (index >= 0 && index < prefabs.Count) 
                {
                    return prefabs[index];
                }

                Debug.LogError(" TilePrefabConfig this [] index is DirectionCount! " + index);
                return null;                
            }
        }

        //等概率通过theme_name获得一个随机的TilePrefabConfig
        public int GetTilePrefabConfigIndexIgnoreOccurancy(string group_name) 
        {
            List<TilePrefabConfig> config_list = null;
			if (prefab_index.TryGetValue(group_name, out config_list) == true && config_list.Count > 0)
            {
                return Random.Range(0, config_list.Count);
            }
            return -1;
        }

        //依据出现概率，通过theme_name获得一个随机的TilePrefabConfig
		public int GetTilePrefabConfigIndex(string group_name)
        {
            List<TilePrefabConfig> config_list = null;
            //存在且不为空
			if (prefab_index.TryGetValue(group_name, out config_list) == true && config_list.Count > 0)
            {
                //暂时不考虑空间换时间，因为还不算太慢
                int sum = 0;
                for (int i = 0; i < config_list.Count; i++)
                {
                    sum += config_list[i].occurancy;
                }

                //按照概率筛选合适的，返回ID
                int chance = Random.Range(0, sum);
                for (int i = 0; i < config_list.Count; i++)
                {
                    if (chance < config_list[i].occurancy)
                    {
                        return config_list[i].index;
                    }
                    chance -= config_list[i].occurancy;
                }
            }

            return -1;
        }


        /// <summary>
        /// 开始之前一定要调用这个接口，否则数据无法被建立起来
        /// </summary>
        public void RebuildTileThemeConfig() 
        {
            prefab_index.Clear();

            for (int i = 0; i < prefabs.Count; i++) 
            {
                prefabs[i].index = i;
                AddTilePrefabConfig(prefabs[i]);
            }
        }

        private void AddTilePrefabConfig(TilePrefabConfig config) 
        {
            if (config == null) 
            {
                return;
            }

            List<TilePrefabConfig> theme_item = null;
			if (prefab_index.TryGetValue(config.group_name, out theme_item) == false) 
            {
                theme_item = new List<TilePrefabConfig>();
				prefab_index[config.group_name] = theme_item;
            }

            theme_item.Add(config);
        }
    }

}
