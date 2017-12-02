using UnityEngine;
using UnityEditor;
using TileMazeMaker.TileGen;

namespace TileMazeMaker 
{

    public class CreateScriptableAsset : MonoBehaviour
    {

        public static T CreateAsset<T>(string path) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return asset;
        }
        
		/* this will be supported in future, but the code is ready, try is if you want.
        [MenuItem("MazeGen/Prefab Based Maze")]
        public static void CreatePrefabBasedMazeConfig()
        {
			CreateAsset<MazeGenerator_Cell_Config>("Assets/TiledMazeMaker/Resources/Configs/");
        }
        */

        [MenuItem("MazeGen/Random Map Config")]
        public static void CreatePerlinMapConfig() 
        {
			CreateAsset<MapGenerator_Perlin_Config>("Assets/TileMazeMaker/Resources/");
        }

        [MenuItem("MazeGen/Maze Map Config")]
        public static void CreateTileBasedMazeMapConfig()
        {
			CreateAsset<MazeGenerator_Tile_Config>("Assets/TileMazeMaker/Resources/");
        }

        [MenuItem("MazeGen/Tile Prefab Config")]
        public static void CreateTilePrefabGroup() 
        {
			CreateAsset<TilePrefabConfig>("Assets/TileMazeMaker/Resources/TileConfigs/");
        }

        [MenuItem("MazeGen/Tile Theme Config")]
        public static void CreateTileThemeGroup() 
        {
			CreateAsset<TileThemeConfig>("Assets/TileMazeMaker/Resources/ThemeConfigs/");
        }

    }
}
