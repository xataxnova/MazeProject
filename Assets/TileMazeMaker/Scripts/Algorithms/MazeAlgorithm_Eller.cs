using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm.Maze
{
    /// <summary>
    /// 对于Eller算法，我做了一些简化。比如第一条。
    /// 在N个数字里面，任意选择几个，进行融合
    /// 我的做法是，对于每一个格子，有50%的概率融合，50%的概率中断融合。结论基本上差不多，但是概率上可能有些出入。
    /// 
    /// 但是一直和书上的形状不太一致。
    /// 
    /// 研究过后，第一点是因为我一开始错误的理解。只能最多有两个格子融合，而不是任意M小于等于N个格子可以融合。
    /// 
    /// 第二点是比较核心的。也就是向下挖洞的时候。
    /// 一行中，独立的组ID，必须向下挖洞。有P个相同组ID的，
    /// 我只选择其中一个向下挖洞。这是不科学的，后来改成，随机Q个格子，向下挖洞，Q大于等于1
    /// 
    /// 最后修改完成之后，觉得这个算法，勉强还能用，但是，太繁琐...
    /// 
    /// 这个算法是结合了SideWinder算法和Kruscal算法的算法。
    /// </summary>
    public class MazeAlgorithm_Eller : MazeAlgorithm_RandomKruskal
    {
        protected override void Generate()
        {
            foreach (var cell in cells) { cell.GroupID = 0; }

            int global_group_id = 1;

            for (int row_id = size_y - 1; row_id >= 0; row_id--) 
            {
                //init_group_id
                for (int ix = 0; ix < size_x; ix++) 
                {
                    IMazeCell cell = GetAt(ix, row_id);
                    if (cell.GroupID == 0) 
                    {
                        cell.GroupID = global_group_id++;
                    }
                }

                if (row_id > 0)
                {
                    //try merge randomly
                    int pointer = 0;
                    while (pointer < size_x - 1)
                    {
                        if (Random.Range(0,2 ) == 0 )
                        {
                            IMazeCell from = GetAt(pointer, row_id);
                            IMazeCell to = GetAt(pointer + 1, row_id);
                            if (from.GroupID != to.GroupID)
                            {
                                Infect(from.GroupID, to);
                                from.ConnectionTo(EMazeDirection.East);
                            }
                        }
                        pointer++;
                    }
                    
                    //try grouth to north
                    List<IMazeCell> temp_list = new List<IMazeCell>();
                    for (int ix = 0; ix < size_x; ix++)
                    {
                        temp_list.Add( GetAt(ix,row_id) );

                        if( ix + 1 == size_x || ( ix+1 < size_x && GetAt(ix, row_id).GroupID != GetAt(ix + 1, row_id).GroupID ))
                        {
                            int rand_times = Random.Range(0, temp_list.Count);
                            if (rand_times == 0) rand_times = 1;

                            while (rand_times > 0) 
                            {
                                IMazeCell rand = temp_list[Random.Range(0, temp_list.Count)];
                                rand.ConnectionTo(EMazeDirection.South);
                                rand.GetNeighbour(EMazeDirection.South).GroupID = rand.GroupID;
                                temp_list.Remove(rand);
                                rand_times--;
                            }
                            
                            temp_list.Clear();
                        }
                    }
                }
                else //编筐编篓，全在收口.
                {
                    //seal all cells

                    for (int ix = 0; ix < size_x - 1; ix++)
                    {
                        IMazeCell from = GetAt(ix, row_id);
                        IMazeCell to = GetAt(ix + 1, row_id);
                        if (from.GroupID != to.GroupID)
                        {
                            from.ConnectionTo(EMazeDirection.East);
                            Infect(from.GroupID, to);
                        }
                    }
                }
            }

            DebugMazeGroupID();
        }
    }
}
