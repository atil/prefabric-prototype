using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using UnityEngine;

namespace Prefabric
{
    // A PlayerPrefs-like usage
    // Better to have it on file

    /// <summary>
    /// This data can be though of parameters to "GameScene's constructor"
    /// I try to make GameScene as seperate from the rest of the game as possible
    /// GameScene is init'ed purely based on GameSceneArgs
    /// </summary>
    public class GameSceneArgs
    {
        /// <summary>
        /// GameScene loads this level, no matter the context
        /// New game, load game, edit mode etc..
        /// </summary>
        public string LevelName { get; private set; }

        /// <summary>
        /// Being in edit mode requires some extra logic in GameScene
        /// </summary>
        public bool IsEditMode { get; private set; }

        /// <summary>
        /// Load data from file and parse it
        /// </summary>
        /// <returns></returns>
        public static GameSceneArgs Load()
        {
            var jsonString = PfResources.LoadStringAt("gameSceneArgs.json");

            // In the first run on a machine, there won't be this json file
            if (jsonString == string.Empty)
            {
                jsonString = GenerateArgsFile();
            }

            var gameSceneArgsJson = JSON.Parse(jsonString);
            return new GameSceneArgs()
            {
                LevelName = gameSceneArgsJson["levelName"].Value,
                IsEditMode = gameSceneArgsJson["isEditMode"].AsBool
            };
        }

        /// <summary>
        /// Save the data to be used init'ing GameScene
        /// </summary>
        /// <param name="lvlName"></param>
        /// <param name="isEditMode"></param>
        public static void Write(string lvlName, bool isEditMode)
        {
            var jsonString = PfResources.LoadStringAt("gameSceneArgs.json");
            
            // In the first run on a machine, there won't be this json file
            if (jsonString == string.Empty)
            {
                jsonString = GenerateArgsFile();
            }

            var gameSceneArgsJson = JSON.Parse(jsonString);
            gameSceneArgsJson["levelName"] = lvlName;
            gameSceneArgsJson["isEditMode"].AsBool = isEditMode;

            System.IO.File.WriteAllText(Util.GetDataPath() + "/gameSceneArgs.json", gameSceneArgsJson.ToString());
        }

        private static string GenerateArgsFile()
        {
            var newJson = new JSONClass();
            newJson["levelName"] = Levels.Paths[0];
            newJson["isEditMode"].AsBool = false;
            System.IO.File.WriteAllText(Util.GetDataPath() + "/gameSceneArgs.json", newJson.ToString());
            return newJson.ToString();
        }
    }
}
