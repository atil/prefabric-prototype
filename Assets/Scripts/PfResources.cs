using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

namespace Prefabric
{
    public enum PfResourceType
    {
        None,
        LevelPathsFile,
        WhiteTile,
        GreyTile,
        BlackTile,
        StartTile,
        EndTile,
        EndZone,
        EditorTileButton,
        EditorFileButton,
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
            ResourcePaths.Add(PfResourceType.GreyTile, "Prefabs/Tiles/GreyTile");
            ResourcePaths.Add(PfResourceType.BlackTile, "Prefabs/Tiles/BlackTile");
            ResourcePaths.Add(PfResourceType.StartTile, "Prefabs/Tiles/StartTile");
            ResourcePaths.Add(PfResourceType.EndTile, "Prefabs/Tiles/EndTile");
            ResourcePaths.Add(PfResourceType.EndZone, "Prefabs/Tiles/EndZone");
            ResourcePaths.Add(PfResourceType.EditorTileButton, "LevelEditor/Prefabs/TileButton");
            ResourcePaths.Add(PfResourceType.EditorFileButton, "LevelEditor/Prefabs/FileButton");
        }

        /// <summary>
        /// Read the file of contents at the given path
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>File contents as string</returns>
        public static string LoadStringAt(string path)
        {
            path = Application.dataPath + "/" + path;
            if (!System.IO.File.Exists(path))
            {
                Debug.LogError("File doesn't exist : " + path);
                return string.Empty;
            }
            return System.IO.File.ReadAllText(path);
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