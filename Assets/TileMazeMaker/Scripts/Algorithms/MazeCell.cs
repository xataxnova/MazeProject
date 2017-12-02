using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm.Maze 
{
    public enum EMazeDirection
    {
        North = 0,
        West = 1,
        South = 2,
        East = 3,
        Invalid = 4,
    }

    public enum ESelectCondition 
    {
        None,
        NoConnected,
        NoVisited,
        Visited,
        Connected,
        VisitedConnected,
    }

    public interface IMazeCell
    {
        int GroupID { get; set; }
        int X { get; set; }
        int Y { get; set; }

        void SetNeighbours(IMazeCell[] cells);

        List<IMazeCell> Neighbours { get; }

        IMazeCell GetNeighbour(EMazeDirection dir);
        EMazeDirection LastRandomNeibourDirection { get; }
        
        EMazeDirection GenRandomNeighbourDirection(ESelectCondition condition = ESelectCondition.None, List<EMazeDirection> direction_filter = null);

        int ConnectionCount { get; }

        /// <summary>
        /// Call this if you need a bi-direction connection.
        /// </summary>
        /// <param name="dir"></param>
        void ConnectionTo(EMazeDirection dir);

        void BuildWallTo(EMazeDirection dir);
        void RemoveConnection(EMazeDirection dir);
        /// <summary>
        /// Call this only if you need a single direction connection!!!!
        /// </summary>
        /// <param name="dir"></param>
        void AddConnection(EMazeDirection dir);
        bool IsConnectedTo(EMazeDirection dir);
        string ToString();
    }

    public class MazeCell : IMazeCell
    {
        public List<EMazeDirection> connections = new List<EMazeDirection>();
        public Dictionary<EMazeDirection, IMazeCell> neighbours = new Dictionary<EMazeDirection, IMazeCell>();

        //cached last random valid direction
        EMazeDirection random_select_direction = EMazeDirection.Invalid;
        public EMazeDirection LastRandomNeibourDirection 
        {
            get 
            {
                return random_select_direction;
            }
        }

        int col;//x_axis
        public int X
        {
            get { return col; }
            set { col = value; }
        }

        int row;//y_axis
        public int Y
        {
            get { return row; }
            set { row = value; }
        }

        int group_id;
        public int GroupID 
        {
            get; 
            set; 
        }

        public List<IMazeCell> Neighbours 
        {
            get 
            {
                return new List<IMazeCell>(neighbours.Values);
            }
        }

        /// <summary>
        /// 必须按照，北，西，南，东的顺序设置。
        /// Must be set with sequence NWSE
        /// </summary>
        /// <param name="neighbours"></param>
        public void SetNeighbours(IMazeCell[] input_neighbours)
        {
            if (input_neighbours.Length != 4)
            {
                return;
            }

            for (int i = 0; i < input_neighbours.Length; i++)
            {
                if (input_neighbours[i] != null)
                {
                    neighbours[(EMazeDirection)i] = input_neighbours[i];
                }
            }

            //Debug.Log(this.ToString() + "Neighbour Count" + neighbours.Count);
        }

        /// <summary>
        /// 返回值可以为Null，null表示邻居为迷宫边界，或是邻居没有被初始化
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>null means no neighbour or not successefully inited.</returns>
        public IMazeCell GetNeighbour(EMazeDirection direction)
        {
            IMazeCell output = null;
            neighbours.TryGetValue(direction, out output);
            return output;
        }

        public EMazeDirection GenRandomNeighbourDirection(ESelectCondition condition = ESelectCondition.None, List<EMazeDirection> direction_filter = null) 
        {
            List<EMazeDirection> candidate = new List<EMazeDirection>();
            foreach (var key in neighbours.Keys) 
            {
                //Debug:注意要判断target是不是visited，而不是自己！
                IMazeCell target = neighbours[key];
                switch( condition )
                {
                    case ESelectCondition.None:
                        candidate.Add( key );
                        break;
                    case ESelectCondition.NoConnected:
                        if( IsConnectedTo( key ) ==  false )
                        {
                            candidate.Add(key);
                        }
                        break;
                    case ESelectCondition.NoVisited:

                        if (target.ConnectionCount == 0)
                        {
                            candidate.Add(key);
                        }
                        break;
                    case ESelectCondition.Connected:
                        if (IsConnectedTo(key) == true) 
                        {
                            candidate.Add(key);
                        }
                        break;
                    case ESelectCondition.Visited:
                        if (target.ConnectionCount > 0) 
                        {
                            candidate.Add(key);
                        }
                        break;
                    case ESelectCondition.VisitedConnected:
                        if (IsConnectedTo(key) == true && target.ConnectionCount > 0) 
                        {
                            candidate.Add(key);
                        }
                        break;
                }
            }

            //追加Filter 使得 随机筛选只能从指定方向中筛选。
            if ( direction_filter != null && candidate.Count > 0 ) 
            {
                for (int i = candidate.Count -1; i >=0 ; i--) 
                {
                    if (direction_filter.Contains(candidate[i]) == false) 
                    {
                        candidate.Remove(candidate[i]);
                    }
                }
            }

            //albour-broder算法需要，因此，得，保存这个值
            random_select_direction = candidate.Count == 0 ? EMazeDirection.Invalid : candidate[Random.Range(0, candidate.Count)];
            return random_select_direction;
        }


        /// <summary>
        /// 获得一个随机的邻居
        /// </summary>
        /// <returns></returns>
        public IMazeCell GetRandomNeighbour()
        {
            int index = Random.Range(0, neighbours.Keys.Count);

            int i = 0;
            foreach (var key in neighbours.Keys)
            {
                if (i == index)
                {
                    random_select_direction = key;
                    break;
                }
                i++;
            }

            return neighbours[random_select_direction];
        }

        public void AddConnection(EMazeDirection dir)
        {
            connections.Add(dir);
        }

        public bool IsConnectedTo(EMazeDirection dir)
        {
            return connections.Contains(dir);
        }

        public void ConnectionTo(EMazeDirection dir) 
        {
            AddConnection(dir);
            neighbours[dir].AddConnection( MazeAlgorithm.InvertDirection(dir));
        }

        public void BuildWallTo(EMazeDirection dir) 
        {
            RemoveConnection(dir);
            neighbours[dir].RemoveConnection(MazeAlgorithm.InvertDirection(dir));
        }

        public void RemoveConnection(EMazeDirection dir) 
        {
            connections.Remove(dir);
        }

        public override string ToString()
        {
            return string.Format("X {0} Y {1}", X, Y);
        }

        public int ConnectionCount
        {
            get
            {
                return connections.Count;
            }
        }


    }

}
