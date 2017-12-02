using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker.Algorithm.Maze 
{

    public class MazeAlgorithm_RecursiveBacktracker : MazeAlgorithm
    {
        protected override bool support_mask
        {
            get
            {
                return true;
            }
        }

        protected override void Generate()
        {
            Stack<IMazeCell> back_tracker = new Stack<IMazeCell>();

            //把起点塞进去
            back_tracker.Push(cells[0]);

            //如果栈空了，表示迷宫完成了。
            while (back_tracker.Count > 0) 
            {
                //寻找栈顶元素
                IMazeCell start_node = back_tracker.Peek();

                //直接选择出，没访问过，且不连通的点。注意，不连通，但是有可能是已经访问过的，没访问过，一定不连通！
                EMazeDirection move_dir = start_node.GenRandomNeighbourDirection( ESelectCondition.NoVisited );

                //如果这个点的周围都已经访问过来，开始回溯，每次遍历回溯一格。直到找到下一个，周围有没访问过的点的节点。
                if (move_dir == EMazeDirection.DirectionCount) 
                {
                    back_tracker.Pop();
                    continue;
                }

                //点周围有没访问过，也没联通的点
                start_node.ConnectionTo(move_dir);
                back_tracker.Push( start_node.GetNeighbour(move_dir) );                
            }
        }
    }

}
