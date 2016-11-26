using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

namespace Prefabric
{
    public enum PfResourceType
    {
        LevelPathsFile,
        WhiteTile,
        StartTile,
        EndTile,
        EditorTileButton,
    }

	public static class PfResources
	{
        private static Dictionary<PfResourceType, string> _resourcePaths = new Dictionary<PfResourceType, string>();

		static PfResources()
        {
            _resourcePaths.Add(PfResourceType.WhiteTile, "Prefabs/Tiles/WhiteTile");
            _resourcePaths.Add(PfResourceType.StartTile, "Prefabs/Tiles/StartTile");
            _resourcePaths.Add(PfResourceType.EndTile, "Prefabs/Tiles/EndTile");
            _resourcePaths.Add(PfResourceType.EditorTileButton, "LevelEditor/Prefabs/TileButton");
        }

        public static string LoadStringAt(string path)
        {
            var textAsset = Resources.Load<TextAsset>(path);
            if (textAsset == null)
            {
                Debug.LogError("No file found for path : " + path);
                return string.Empty;
            }
            return textAsset.text;
        }

        public static T Load<T>(PfResourceType resType) where T : Object
        {
            if (!_resourcePaths.ContainsKey(resType))
            {
                Debug.LogError("Resource doesn't exist : " + resType);
                return null;
            }

            return Resources.Load<T>(_resourcePaths[resType]);
        }
	}
}