using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker 
{
    [System.Serializable]
    public abstract class GameConfigTemplate<KEY,USER_TYPE> : ScriptableObject
    {
        public List<USER_TYPE> data = new List<USER_TYPE>();        
        protected Dictionary<KEY, USER_TYPE> data_index = new Dictionary<KEY, USER_TYPE>();

        /// <summary>
        /// 添加一个空元素
        /// </summary>
        virtual public void AddEmpty() 
        {
            data.Add( System.Activator.CreateInstance<USER_TYPE>() );
        }

        /// <summary>
        /// 删除指定位置上的可配置元素
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index) 
        {
            if (index >= 0 && index < Count) 
            {
                data.RemoveAt(index);
            }
        }

        /// <summary>
        /// 配置表元素个数
        /// </summary>
        public int Count 
        {
            get 
            {
                return data.Count;
            }
        }

        abstract public void RebuildIndex();

        public USER_TYPE GetValueAt(int index) 
        {
            return data[index];
        }

        public void SetValueAt(int index, USER_TYPE val) 
        {
            data[index] = val;
        }

        public USER_TYPE this[KEY key] 
        {
            get 
            {
                return data_index[key];
            }
        }
    }

}
