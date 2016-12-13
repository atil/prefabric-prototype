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
    public class GameSceneArgs
    {
        public string LevelName { get; private set; }

        public static GameSceneArgs Load()
        {
            var gameSceneArgsJson = JSON.Parse(PfResources.LoadStringAt("gameSceneArgs.json"));
            return new GameSceneArgs()
            {
                LevelName = gameSceneArgsJson["levelName"].Value + ".json"
            };
        }

        public static void Write(string lvlName)
        {
            var gameSceneArgsJson = JSON.Parse(PfResources.LoadStringAt("gameSceneArgs.json"));
            gameSceneArgsJson["levelPath"] = lvlName;
            System.IO.File.WriteAllText(Application.dataPath + "/Resources/gameSceneArgs.json", gameSceneArgsJson.ToString());
        }
    }
}
