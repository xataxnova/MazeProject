using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TileMazeMaker.TileGen
{
    using TileMazeMaker;
    using TileMazeMaker.Algorithm.Maze;

    public enum EMazePrefabType
    {
        Cross,
        U,
        L,
        I,
        T,
    }

    [System.Serializable]
    public class MazePrefab
    {
        public EMazePrefabType maze_cell_type = EMazePrefabType.Cross;
        public List<EMazeDirection> match_direction = new List<EMazeDirection>();
        [SerializeField]
        private List<GameObject> variables = new List<GameObject>();

        public GameObject prefab
        {
            get
            {
                return variables[Random.Range(0, variables.Count)];
            }
        }
    }

    public class MazeGenerator_Cell : MonoBehaviour,IMapGenerator
    {
        [SerializeField]
        private Transform map_root;
        [SerializeField]
        private MazeGenerator_Cell_Config config;
        
        public void BuildMap()
        {
            if (config.width > 2 && config.height > 2)
            {
                this.DestroyAllChild();
                MazeAlgorithm algorithm = (MazeAlgorithm)System.Activator.CreateInstance(System.Type.GetType("TileMazeMaker.Algorithm.Maze." + config.aglorithm.ToString()));
                algorithm.BuildMaze<MazeCell>(config.width, config.height);

                for (int iy = 0; iy < config.height; iy++)
                {
                    for (int ix = 0; ix < config.width; ix++)
                    {
                        CreateMazeCellObject(algorithm.GetAt(ix, iy));
                    }
                }
            }
            else
            {
                Debug.Log("Maze should bigger than 2X2");
            }
        }



        GameObject CreateMazeCellObject(IMazeCell cell_def)
        {
            List<EMazeDirection> connections = new List<EMazeDirection>();
            for (int i = 0; i < (int)EMazeDirection.DirectionCount; i++)
            {
                if (cell_def.IsConnectedTo((EMazeDirection)i))
                {
                    connections.Add((EMazeDirection)i);
                }
            }

            MazePrefab match = null;

            foreach (var candidate in config.cell_list)
            {
                if (candidate.match_direction.Count == connections.Count)
                {
                    bool full_match = true;
                    foreach (var test_dir in connections)
                    {
                        if (candidate.match_direction.Contains(test_dir) == false)
                        {
                            full_match = false;
                            break;
                        }
                    }

                    if (full_match)
                    {
                        //match 
                        match = candidate;
                        break;
                    }
                }
            }

            GameObject result = Object.Instantiate(match.prefab);
            result.transform.SetParent(map_root);
            result.transform.localPosition = new Vector3(cell_def.X * config.grid_size, 0, cell_def.Y * config.grid_size);

            return result;
        }


        public void ClearMap()
        {
            map_root.DestroyAllChild();
        }
    }

}