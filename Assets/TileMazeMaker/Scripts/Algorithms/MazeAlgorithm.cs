using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileMazeMaker;

namespace TileMazeMaker.Algorithm.Maze 
{
    public enum EMazeMaskOptions
    {
        None,
        MaskArray,
    }


    public enum EMazeAlgorithm
    {
        MazeAlgorithm_BinaryTree,
        MazeAlgorithm_Sidewinder,
        MazeAlgorithm_AldourBroder,
        MazeAlgorithm_Wilson,
        MazeAlgorithm_AldourBroder_Wilson,
        MazeAlgorighm_HuntAndKill,
        MazeAlgorithm_RecursiveBacktracker,
        MazeAlgorithm_RandomKruskal,
        MazeAlgorithm_SimplePlim,
        MazeAlgorithm_TruePlim,
        MazeAlgorithm_GrowingTree,
        MazeAlgorithm_Eller,
        MazeAlgorithm_RecursiveDivision,
    }

    public enum EMazePostProcess
    {
        None,
        MazePostProcess_Braiding,
        MazePostProcess_OptmizedBraiding,
    }

    /// <summary>
    /// 大重构，让算法真正和显示逻辑分离，以后无论是想要显示成TileMap还是显示成Texture都全部OK，甚至生成CustomMesh也都OK
    /// </summary>
    public abstract class MazeAlgorithm
    {
        //用Dictionary会不会更合适，如果是不规则迷宫的话
        protected List<IMazeCell> cells = new List<IMazeCell>();
        
        public int size_x;
        public int size_y;

        List<int> maze_mask_cells;

        virtual protected bool support_mask 
        {
            get 
            {
                return false;
            }
        }

        public void SetMazeMaskCells(List<int> maze_mask_cells) 
        {
            if (support_mask)
            {
                this.maze_mask_cells = maze_mask_cells;
            }
            else 
            {
                this.maze_mask_cells = null;
            }            
        }

        public void BuildMaze<T>(int x_size, int y_size) where T:IMazeCell
        {
            this.size_x = x_size;
            this.size_y = y_size;

            cells.Capacity = size_x * size_y;

            int group_id = 0;

            //FIRST PASS
            for (int iy = 0; iy < size_y; iy++)
            {
                for (int ix = 0; ix < size_x; ix++)
                {
                    if (maze_mask_cells != null && maze_mask_cells.Contains( SharedUtil.PointHash(ix, iy)))
                    {
                        cells.Add(null);
                    }
                    else 
                    {
                        T cell = System.Activator.CreateInstance<T>();
                        cell.X = ix;
                        cell.Y = iy;
                        cell.GroupID = group_id++;
                        cells.Add(cell);
                    }
                }
            }

            //SECOND PASS
            for (int iy = 0; iy < size_y; iy++)
            {
                for (int ix = 0; ix < size_x; ix++)
                {
                    IMazeCell cell = GetAt(ix, iy);
                    if (cell != null) 
                    {
                        IMazeCell[] neighbours = new IMazeCell[4];
                        neighbours[(int)EMazeDirection.North] = GetAt(ix, iy + 1);
                        neighbours[(int)EMazeDirection.South] = GetAt(ix, iy - 1);
                        neighbours[(int)EMazeDirection.West] = GetAt(ix - 1, iy);
                        neighbours[(int)EMazeDirection.East] = GetAt(ix + 1, iy);
                        cell.SetNeighbours(neighbours);
                    }                    
                }
            }

            Generate();

            for (int i = 0; i < post_processer.Count; i++) 
            {
                if (post_processer[i] != null) 
                {
                    post_processer[i].PostProcess(this);
                }
            }
        }

        public IMazeCell GetAt(int x, int y)
        {
            if (y >= 0 && y < size_y && x >= 0 && x < size_x)
            {
                return cells[y * size_x + x];
            }
            return null;
        }

        protected IMazeCell GetRandomCell() 
        {
            return cells[Random.Range(0, cells.Count)];
        }

        abstract protected void Generate();

        public int DeadEndCount 
        {
            get 
            {
                int sum = 0;
                foreach (var cell in cells) 
                {
                    if (cell != null && cell.ConnectionCount == 1) 
                    {
                        sum++;
                    }
                }

                return sum;
            }
        }

        public override string ToString()
        {
            return string.Format( "Maze {0} Dead Ends {1}", GetType(), DeadEndCount );
        }

        List<MazePostProcesser> post_processer = new List<MazePostProcesser>();
        public void AddPostProcesser( string refect_calss ) 
        {
            MazePostProcesser mpp = (MazePostProcesser)System.Activator.CreateInstance(System.Type.GetType("TileMazeMaker.Algorithm.Maze." + refect_calss));
            post_processer.Add(mpp);
        }

        public static EMazeDirection CWNextDirection(EMazeDirection direction)
        {
            if (direction == EMazeDirection.East) return EMazeDirection.North;
            if (direction == EMazeDirection.North) return EMazeDirection.West;
            if (direction == EMazeDirection.West) return EMazeDirection.South;
            return EMazeDirection.East;
        }

        public static EMazeDirection CCWNextDirection(EMazeDirection direction)
        {
            if (direction == EMazeDirection.East) return EMazeDirection.South;
            if (direction == EMazeDirection.North) return EMazeDirection.East;
            if (direction == EMazeDirection.West) return EMazeDirection.North;
            return EMazeDirection.West;
        }

        public static EMazeDirection InvertDirection(EMazeDirection direction)
        {
            if (direction == EMazeDirection.East) return EMazeDirection.West;
            if (direction == EMazeDirection.North) return EMazeDirection.South;
            if (direction == EMazeDirection.West) return EMazeDirection.East;
            return EMazeDirection.North;
        }
    }

    public abstract class MazePostProcesser 
    {
        abstract public void PostProcess(MazeAlgorithm maze_algorithm);
    }


}
