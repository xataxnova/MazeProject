using UnityEngine;

namespace TileMazeMaker.TileGen
{
    public interface iResourceProvider
    {
        Object LoadObject(string path_name);
    }

    public interface iResourceLoader
    {
        void Reset(iResourceProvider provider = null);
        GameObject GetTileObject(TilePrefabConfig config);
        void RecycleGameObject(GameObject prefab_instance);
    }
}
