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
        public static void BuildLevelEditor()
        {
            Build("PrefabEd", new []
            {
                "Assets/Scenes/LevelEditorScene.unity",
                "Assets/Scenes/GameScene.unity",
            }, 
            BuildTarget.StandaloneWindows,
            BuildOptions.Development | BuildOptions.AllowDebugging);
        }

        [MenuItem("Prefabric/Build Game", false, 11)]
        public static void BuildGame()
        {
            Build("Prefabric", new[]
            {
                "Assets/Scenes/EntryScene.unity",
                "Assets/Scenes/MainMenuScene.unity",
                "Assets/Scenes/LevelEditorScene.unity",
                "Assets/Scenes/GameScene.unity",
            }, 
            BuildTarget.StandaloneWindows,
            BuildOptions.Development | BuildOptions.AllowDebugging);
        }

        [MenuItem("Prefabric/Build Android", false, 11)]
        public static void BuildAndroid()
        {
            Build("Prefabric", new[]
            {
                "Assets/Scenes/EntryScene.unity",
                "Assets/Scenes/MainMenuScene.unity",
                "Assets/Scenes/LevelEditorScene.unity",
                "Assets/Scenes/GameScene.unity",
            }, BuildTarget.Android,
            BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.AutoRunPlayer);
        }

        private static void Build(string buildName, string[] scenes, BuildTarget target, BuildOptions ops)
        {
            var buildPath = EditorUtility.SaveFolderPanel("Choose Location ", "C:/asd/build/", buildName);
            var buildDataPath = target != BuildTarget.Android
                ? buildPath + "/" + buildName + "_Data/" 
                : Application.persistentDataPath;

            if (string.IsNullOrEmpty(buildPath))
            {
                return;
            }

            BuildPipeline.BuildPlayer(scenes, buildPath + "/" + buildName + ".exe", target, ops);

            // Copy levels
            var levelsPath = Application.dataPath + "/Levels/";
            var levelsPathTarget = Path.Combine(buildDataPath, "/Levels/");
            DirectoryCopy(levelsPath, levelsPathTarget);

            FileUtil.ReplaceDirectory(levelsPath, levelsPathTarget);

            //Directory.CreateDirectory(Application.persistentDataPath + "/Test1");
            //Directory.CreateDirectory(Application.persistentDataPath + "Test2");

            // Copy gameSceneArgs file
            var gameSceneArgsPath = Application.dataPath + "/gameSceneArgs.json";
            var gameSceneArgsPathTarget = buildDataPath + "/gameSceneArgs.json";
            if (!File.Exists(gameSceneArgsPathTarget))
            {
                File.Copy(gameSceneArgsPath, gameSceneArgsPathTarget);
            }
            // Copy levelPaths file
            //var levelPathsPath = Application.dataPath + "/levelPaths.json";
            //var levelPathsPathTarget = buildDataPath + "levelPaths.json";
            //if (!File.Exists(levelPathsPathTarget))
            //{
            //    File.Copy(levelPathsPath, levelPathsPathTarget);
            //}

        }

        private static void DirectoryCopy(string sourceDirName, string destDirName) // From MSDN
        {
            //Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            //// Get the files in the directory and copy them to the new location.
            //var files = dir.GetFiles().Where(x => !x.Extension.EndsWith("meta"));
            //foreach (var file in files)
            //{
            //    var temppath = Path.Combine(destDirName, file.Name);
            //    try
            //    {
            //        file.CopyTo(temppath, false);
            //    }
            //    catch (IOException e)
            //    {
            //        // Supress 'file already exists' error
            //    }
            //}

            //// If copying subdirectories, copy them and their contents to new location. 
            //var dirs = dir.GetDirectories();
            //foreach (var subdir in dirs)
            //{
            //    var temppath = Path.Combine(destDirName, subdir.Name);
            //    DirectoryCopy(subdir.FullName, temppath);
            //}
        }

    }
}
#endif