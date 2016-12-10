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
        EndZone,
        EditorTileButton,
    }

    /// <summary>
    /// An abstraction layer for loading resources from files
    /// Built around a "resourcesType -> path" mapping
    /// The good is, there'll be no paths embedded in the code (except here),
    /// and these paths can be read from a file as well, if rebuilding the project becomes an issue
    /// The bad is, it adds another step of work when a resource is needed
    /// </summary>
	public static class PfResources
	{
        private static readonly Dictionary<PfResourceType, string> ResourcePaths = new Dictionary<PfResourceType, string>();

		static PfResources()
        {
            ResourcePaths.Add(PfResourceType.WhiteTile, "Prefabs/Tiles/WhiteTile");
            ResourcePaths.Add(PfResourceType.StartTile, "Prefabs/Tiles/StartTile");
            ResourcePaths.Add(PfResourceType.EndTile, "Prefabs/Tiles/EndTile");
            ResourcePaths.Add(PfResourceType.EndZone, "Prefabs/Tiles/EndZone");
            ResourcePaths.Add(PfResourceType.EditorTileButton, "LevelEditor/Prefabs/TileButton");
        }

        /// <summary>
        /// Read the file of contents at the given path
        /// The file must be under a "Resources/" directory
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>File contents as string</returns>
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

        /// <summary>
        /// A wrapper around Unity's Resources.Load() call
        /// </summary>
        /// <typeparam name="T">System.Type of resource</typeparam>
        /// <param name="resType">User-defined resource type</param>
        /// <returns>Resource instance</returns>
        public static T Load<T>(PfResourceType resType) where T : Object
        {
            if (!ResourcePaths.ContainsKey(resType))
            {
                Debug.LogError("Resource doesn't exist : " + resType);
                return null;
            }

            return Resources.Load<T>(ResourcePaths[resType]);
        }
	}
}