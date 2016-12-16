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
            var gameSceneArgsJson = JSON.Parse(PfResources.LoadStringAt("gameSceneArgs.json"));
            return new GameSceneArgs()
            {
                LevelName = gameSceneArgsJson["levelName"].Value + ".json",
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
            var gameSceneArgsJson = JSON.Parse(PfResources.LoadStringAt("gameSceneArgs.json"));
            gameSceneArgsJson["levelPath"] = lvlName;
            gameSceneArgsJson["isEditMode"].AsBool = isEditMode;
            System.IO.File.WriteAllText(Application.dataPath + "/Resources/gameSceneArgs.json", gameSceneArgsJson.ToString());
        }
    }
}
