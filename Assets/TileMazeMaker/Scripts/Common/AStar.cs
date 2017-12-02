using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm 
{
    public interface IAStarNode
    {
        //获得邻居节点的接口，这样就不限于只能移动上下左右了。
        IAStarNode[] Neibhours { get; }

        //获得点的X坐标
        int X { get; }
        //获得点的Y坐标
        int Y { get; }

        //EG：曼哈顿距离*10
        void CalculeteH(IAStarNode dest);

        //EG：水平垂直每次加10，斜角每次加14等（约等于根号200）
        int G { get; set; }

        /// <summary>
        /// 这样做是为了方便计算，斜角的时候，的GWeight
        /// </summary>
        /// <param name="target_node"></param>
        /// <returns></returns>
        int GWeightTo(IAStarNode target_node);

        //F = G + H
        int F { get; }

        bool Walkable { get; }

        IAStarNode ParentNode { get; set; }
    }

    public class AStar
    {
        IAStarNode start;
        IAStarNode destination;
        IAStarNode active_node;
        List<IAStarNode> open_list;
        List<IAStarNode> close_list;
        List<IAStarNode> path;

        void WarmUp(IAStarNode start_node, IAStarNode end_node)
        {
            start = start_node;
            destination = end_node;
            open_list = new List<IAStarNode>();
            close_list = new List<IAStarNode>();
            path = new List<IAStarNode>();

            //第一个节点。加入Open表。
            AddToOpenList(start, null);//父节点为Null，代表的就是起始节点！        
        }

        /// <summary>
        /// 加入Open表
        /// 计算F值，一次就够了。
        /// 添加ParentNode，一步完成，防止错误。
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent_node"></param>
        void AddToOpenList(IAStarNode node, IAStarNode parent_node)
        {
            open_list.Add(node);
            node.CalculeteH(destination);
            node.ParentNode = parent_node;
        }

        /// <summary>
        /// 选择F值最小的，作为ActiveNode
        /// </summary>
        /// <returns>Continue or not,true continue, false path found!</returns>
        bool SelectMinF()
        {
            IAStarNode new_active_node = open_list[0];

            if (open_list.Count > 1)
            {
                for (int i = 1; i < open_list.Count; i++)
                {
                    if (new_active_node.F > open_list[i].F)
                    {
                        new_active_node = open_list[i];
                    }
                }
            }

            //删除将要返回的node。
            open_list.Remove(new_active_node);
            close_list.Add(new_active_node);
            active_node = new_active_node;

            if (active_node == destination)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 如果不可达，返回null
        /// 如果可达，返回路径。
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<IAStarNode> GetPathTo(IAStarNode start, IAStarNode end)
        {
            //Debug.Log(" Start " + start + "end" + end);

            WarmUp(start, end);


            int dead_lock = 0;

            while (open_list.Count > 0)
            {
                if (SelectMinF() == true)
                {
                    IAStarNode[] neighours = active_node.Neibhours;
                    for (int i = 0; i < neighours.Length; i++)
                    {
                        if (neighours[i] != null && neighours[i].Walkable && close_list.Contains(neighours[i]) == false)
                        {
                            int new_G_weight = active_node.G + neighours[i].GWeightTo(active_node);

                            //如果邻居不在Open表中，那么直接将邻居加入Open表，并将active_node设置为其父节点。
                            if (open_list.Contains(neighours[i]) == false)
                            {
                                neighours[i].G = new_G_weight;
                                AddToOpenList(neighours[i], active_node);
                            }
                            else//
                            {
                                if (neighours[i].G < new_G_weight)
                                {
                                    neighours[i].G = new_G_weight;
                                    neighours[i].ParentNode = active_node;
                                }
                            }
                        }
                    }
                }
                else
                {
                    int trace_path_lock = 0;

                    //找到了
                    while (active_node != start)
                    {
                        path.Add(active_node);

                        //Debug.Log(string.Format("Path add {0}-{1}", active_node.X, active_node.Y));

                        active_node = active_node.ParentNode;

                        trace_path_lock++;
                        if (trace_path_lock > 2000)
                        {
                            break;
                        }
                    }

                    path.Add(start);
                    //Debug.Log(string.Format("Path add {0}-{1}", start.X, start.Y));

                    path.Reverse();
                    return path;
                }

                //外层死循环安全锁
                dead_lock++;
                if (dead_lock > 100000)
                {
                    break;
                }
            }

            return null;//没找到        
        }
    }

}
