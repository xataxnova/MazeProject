using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm.Maze 
{

    public class MazeAlgorithm_RandomKruskal : MazeAlgorithm
    {
        /// <summary>
        /// 研究这个算法的时候，遇到了一些坑
        /// 主要是我想偷懒，一开始，没用到记录使用过的 连接点的策略
        /// 而是试图，用算法算出来
        /// 但是，不论通过：A - 统计连接，B-统计GroupID是否一致，都无法正确的算出一个点，是否被用过。
        /// 如果
        /// 
        /// 1|1
        /// 1 1 
        /// 
        /// 用GroupID来判断。很有可能一个没用过的点，被当成用过的了。错误
        /// 用Connection来判断。一个已经用过的点，会反复被当成没用过的来多次计算。
        /// 于是，最终还是回到了相对浪费空间的一种算法。那就是
        /// 
        /// 用两个Int型数组，记录所有使用过的连接点的 Hash结果。然后下次寻找的时候，发现已经用过的，直接跨过。
        /// 事实上我们必须这么做，否则就算你记录每个Group为一个List，也不能保证不犯上面的错误。除非，你记录两套GroupLists
        /// 一套为水平扫描准备。
        /// 一套为垂直扫描准备。
        /// 但是这两套，空间消耗都太大，并且反复创建List，GC开销也是蛮大的。所以最终结果，还是用现在的方式。
        /// 
        /// 分析一下，这种算法不是使用随机连接某个点的方式计算迷宫的，所以不适合任意形状的迷宫
        /// 但是，他特别适合生成 Waving Maze
        /// </summary>
        protected override void Generate()
        {
            int horizon_merge = size_x - 1;
            int vertical_merge = size_y - 1;

            List<int> used_horizon = new List<int>();
            List<int> used_vertical = new List<int>();

            //先横向融合，再纵向融合，为一个Loop，知道所有的格子都被合在一起。
            while (horizon_merge > 0 || vertical_merge > 0) 
            {                               
                if (horizon_merge > 0) 
                {                    
                    //horizon merge,loop every row
                    for (int i = 0; i < size_y; i++)
                    {
                        int rand_num = Random.Range(0, horizon_merge);
                        IMazeCell start_merge = FindRandomMergeStartCell(rand_num, true,i, used_horizon);
                        used_horizon.Add( SharedUtil.PointHash(start_merge.X, start_merge.Y));

                        if (start_merge.GroupID != start_merge.GetNeighbour(EMazeDirection.East).GroupID) 
                        {
                            Infect(start_merge.GroupID, start_merge.GetNeighbour(EMazeDirection.East));
                            start_merge.ConnectionTo(EMazeDirection.East);
                        }
                    }
                    horizon_merge--;
                }

                if (vertical_merge > 0)
                {
                    for (int i = 0; i < size_x; i++)
                    {
                        int rand_num = Random.Range(0, vertical_merge);
                        IMazeCell start_merge = FindRandomMergeStartCell(rand_num, false, i, used_vertical);
                        used_vertical.Add(SharedUtil.PointHash(start_merge.X, start_merge.Y));

                        if (start_merge.GroupID != start_merge.GetNeighbour(EMazeDirection.North).GroupID)
                        {
                            Infect(start_merge.GroupID, start_merge.GetNeighbour(EMazeDirection.North));
                            start_merge.ConnectionTo(EMazeDirection.North);
                        }
                    }
                    vertical_merge--;
                }
            }
        }

        protected void DebugMazeGroupID()
        {
            Debug.Log("-------------------------------------");
            for (int iy = size_y-1; iy >= 0; iy--) 
            {
                string row_string = "";
                for (int ix = 0; ix < size_x; ix++) 
                {
                    row_string += GetAt(ix, iy).GroupID + ",";
                }
                Debug.Log(row_string);
            }
        }

        /// <summary>
        /// 寻找一个随机的，还没用过的，连接点
        /// </summary>
        /// <param name="rand_num">随机数，随机数越来越小，因为可选择的随机起始点，会随着融合越来越少</param>
        /// <param name="is_row">true:水平。false:垂直</param>
        /// <param name="index">is_row:true 传入行索引（y），否则传入列索引（x）</param>
        /// <param name="used"> is_row:true 传入用过的水平连接点。否则传入用过的垂直连接点。</param>
        /// <returns>随机的，没用过的连接点</returns>
        IMazeCell FindRandomMergeStartCell(int rand_num, bool is_row,int index,List<int> used) 
        {
            IMazeCell start_cell = is_row ? GetAt(0, index) : GetAt(index, 0);
            EMazeDirection dir = is_row ? EMazeDirection.East : EMazeDirection.North;
            
            do
            {
                while ( used.Contains( SharedUtil.PointHash( start_cell.X,start_cell.Y ) ) )
                {
                    start_cell = start_cell.GetNeighbour(dir);
                }

                if (rand_num == 0)
                {
                    return start_cell;
                }

                start_cell = start_cell.GetNeighbour(dir);
                rand_num--;
            }
            while (rand_num >= 0);

            Debug.LogError("Error this should not happen!!");
            return null;
        }

        protected void Infect(int new_group_id, IMazeCell to) 
        {
            int old_group_id = to.GroupID;
            Stack<IMazeCell> infect_stack = new Stack<IMazeCell>();
            to.GroupID = new_group_id;
            infect_stack.Push( to );

            //if stack is not null.
            while (infect_stack.Count > 0) 
            {
                //find stack top
                IMazeCell top = infect_stack.Pop();
                List<IMazeCell> neighbours = top.Neighbours;

                //for all top`s neighbours.
                for (int i = 0; i < neighbours.Count; i++) 
                {
                    //if belong to old group.
                    if (neighbours[i].GroupID == old_group_id) 
                    {
                        //merge to new group go on to find with it`s neighbours untill no new neighbour is added.
                        neighbours[i].GroupID = new_group_id;
                        infect_stack.Push(neighbours[i]);
                    }
                }
            }
        }
    }

}
