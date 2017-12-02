using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker 
{
    /// <summary>
    /// 用于计算Dijkstra路径的节点。
    /// </summary>
    public interface IDijkstraNode 
    {
        IDijkstraNode[] connections
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 荷兰数学家迪杰斯特拉的经典寻路算法。为啥迷宫寻路不适合AStar?
    /// 迷宫寻路用AStar效率没有dijkstra好嘛？
    /// 嗷嗷嗷嗷嗷
    /// 嗷嗷嗷嗷嗷嗷嗷嗷
    /// 
    /// 
    /// </summary>
    public class Dijkstra 
    {
        public class Distances 
        {
            public IDijkstraNode root;
            public Dictionary<IDijkstraNode, int> distance_table;

            public Distances(IDijkstraNode root) 
            {
                this.root = root;
                distance_table = new Dictionary<IDijkstraNode, int>();
                distance_table[root] = 0;
            }

            public int GetDistance(IDijkstraNode target) 
            {
                return distance_table[target];
            }

            public bool Visited(IDijkstraNode target) 
            {
                return distance_table.ContainsKey(target);
            }

            public void SetDistanceReferenceTo(IDijkstraNode target, IDijkstraNode reference)
            {
                distance_table[target] = distance_table[reference] + 1;
            }

            public List<IDijkstraNode> GetPath(IDijkstraNode end) 
            {
                if( Visited(end) == false) return null;
                
                List<IDijkstraNode> result = new List<IDijkstraNode>();
                result.Add(end);
                while (result[result.Count - 1] != root) 
                {
                    IDijkstraNode prev = null;
                    int min_dist = distance_table[result[result.Count - 1]];
                    
                    foreach (var conn in end.connections) 
                    {
                        int prev_dist = distance_table[conn];
                        if (prev_dist < min_dist) 
                        {
                            min_dist = prev_dist;
                            prev = conn;
                        }
                    }
                    result.Add(prev);
                }

                result.Reverse();

                return result;
            }

            public IDijkstraNode LongistNode() 
            {
                IDijkstraNode longest_node = null;
                int longest = 0;

                foreach (var node in distance_table.Keys) 
                {
                    if (distance_table[node] > longest) 
                    {
                        longest = distance_table[node];
                        longest_node = node;
                    }
                }

                return longest_node;
            }
        }

        Distances diss;

        /// <summary>
        /// 这个方法有很多用途
        /// 【1】寻找路径
        /// 【2】寻找最优出入口
        /// 【3】给迷宫上色（依据距离某一个点的距离）
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        Distances FloodMaze(IDijkstraNode start) 
        {
            Distances diss = new Distances(start);

            List<IDijkstraNode> frontiers = new List<IDijkstraNode>();
            frontiers.Add(start);

            //Algorithm from the book Mazes for programmers.
            while (frontiers.Count > 0)
            {
                List<IDijkstraNode> new_frontiers = new List<IDijkstraNode>();

                foreach (var front in frontiers)
                {
                    foreach (var conn in front.connections)
                    {
                        if (diss.Visited(conn) == false)
                        {
                            diss.SetDistanceReferenceTo(conn, front);
                            new_frontiers.Add(conn);
                        }
                    }
                }

                frontiers = new_frontiers;
            }

            return diss;
        }

        //完美迷宫的简化Dijkstra算法。
        //因为完美迷宫不会有闭环，
        //因为完美迷宫任意两个点之间有且只有一条路径，所以，不需要考虑比较路径，和优化路径的问题。找到的节点一定是目的地节点。
        //这个应该是，全覆盖的Dijkstra算法。
        List<IDijkstraNode> GetDijkstraInPerfectMaze(IDijkstraNode start, IDijkstraNode end) 
        {
            return FloodMaze(start).GetPath(end);
        }

        List<IDijkstraNode> LongestPathInMaze(IDijkstraNode start)
        {
            Distances diss = FloodMaze(start);
            //寻找第一个最远点。
            IDijkstraNode max_start = diss.LongistNode();

            //从第一个最远点，作为起点，获得第二个Flood结果，计算出所有点相对于这个点的距离。
            Distances diss_final = FloodMaze(max_start);

            //或的相对第一个最远点，新的最远点。
            IDijkstraNode max_end = diss.LongistNode();

            //算出这个点，到第一个最远点的路径，就是最长路径。
            return diss_final.GetPath(max_end);
        }
    }

}
