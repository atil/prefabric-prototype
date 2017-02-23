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
        EditorTileButton,
        EditorFileButton,
        Player,
        BendGuide,
        SfxClick1,
        SfxClick2,
        SfxMusic1,
        SfxMusic2,
        SfxMusic3,
        SfxTileSelect1,
        SfxTileSelect2,
        SfxBendFail,
        SfxUnbend,
        CursorImage,
    }

    /// <summary>
    /// An abstraction layer for loading resources from files
    /// Built around a "resourceType -> path" mapping
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
            ResourcePaths.Add(PfResourceType.EditorTileButton, "LevelEditor/Prefabs/TileButton");
            ResourcePaths.Add(PfResourceType.EditorFileButton, "LevelEditor/Prefabs/FileButton");
            ResourcePaths.Add(PfResourceType.Player, "Prefabs/Player");
            ResourcePaths.Add(PfResourceType.BendGuide, "Prefabs/BendGuide");
            ResourcePaths.Add(PfResourceType.SfxClick1, "Sfx/Click1");
            ResourcePaths.Add(PfResourceType.SfxClick2, "Sfx/Click2");
            ResourcePaths.Add(PfResourceType.SfxMusic1, "Sfx/Music_Floating_Cities");
            ResourcePaths.Add(PfResourceType.SfxMusic2, "Sfx/Music_Soaring");
            ResourcePaths.Add(PfResourceType.SfxMusic3, "Sfx/Music_Overheat");
            ResourcePaths.Add(PfResourceType.SfxTileSelect1, "Sfx/TileSelect1");
            ResourcePaths.Add(PfResourceType.SfxTileSelect2, "Sfx/TileSelect2");
            ResourcePaths.Add(PfResourceType.SfxBendFail, "Sfx/BendFail");
            ResourcePaths.Add(PfResourceType.SfxUnbend, "Sfx/Unbend");
            ResourcePaths.Add(PfResourceType.CursorImage, "Textures/Cursor");
        }

        /// <summary>
        /// Read the file of contents at the given path
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>File contents as string</returns>
        public static string LoadStringAt(string path)
        {
            path = Util.GetDataPath() + "/" + path;
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