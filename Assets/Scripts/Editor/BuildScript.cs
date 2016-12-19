#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Prefabric
{
    public class BuildScript
    {
        [MenuItem("Prefabric/Build Level Editor", false, 11)]
        public static void BuildStandalone()
        {
            const string buildName = "PrefabEd";
            var buildPath = EditorUtility.SaveFolderPanel("Choose Location ", "C:/asd/build/", buildName);
            var buildDataPath = buildPath + "/" + buildName + "_Data/";

            if (string.IsNullOrEmpty(buildPath))
            {
                return;
            }

            string[] levels =
            {
                "Assets/Scenes/LevelEditorScene.unity",
                "Assets/Scenes/GameScene.unity",
            };

            const BuildOptions ops = BuildOptions.Development | BuildOptions.AllowDebugging;
            BuildPipeline.BuildPlayer(levels, buildPath + "/" + buildName + ".exe", BuildTarget.StandaloneWindows, ops);

            // Copy gameSceneArgs file
            var gameSceneArgsPath = Application.dataPath + "/gameSceneArgs.json";
            var gameSceneArgsPathTarget = buildDataPath + "gameSceneArgs.json";
            if (!File.Exists(gameSceneArgsPathTarget))
            {
                File.Copy(gameSceneArgsPath, gameSceneArgsPathTarget);
            }

            // Copy levelPaths file
            var levelPathsPath = Application.dataPath + "/levelPaths.json";
            var levelPathsPathTarget = buildDataPath + "levelPaths.json";
            if (!File.Exists(levelPathsPathTarget))
            {
                File.Copy(levelPathsPath, levelPathsPathTarget);
            }

            // Copy levels
            var levelsPath = Application.dataPath + "/Levels/";
            var levelsPathTarget = buildDataPath + "/Levels";
            DirectoryCopy(levelsPath, levelsPathTarget);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName) // From MSDN
        {
            //Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);
            var dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles().Where(x => !x.Extension.EndsWith("meta"));
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);
                try
                {
                    file.CopyTo(temppath, false);
                }
                catch (IOException e)
                {
                    // Supress 'file already exists' error
                }
            }

            // If copying subdirectories, copy them and their contents to new location. 
            foreach (var subdir in dirs)
            {
                var temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }

    }
}
#endif