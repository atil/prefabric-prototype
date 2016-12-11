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
            var gameSceneArgsJson = JSON.Parse(PfResources.LoadStringAt("gameSceneArgs"));
            return new GameSceneArgs()
            {
                LevelName = gameSceneArgsJson["levelName"].Value
            };
        }

        public static void Write(string lvlName)
        {
            var gameSceneArgsJson = JSON.Parse(PfResources.LoadStringAt("gameSceneArgs"));
            gameSceneArgsJson["levelPath"] = lvlName;
            System.IO.File.WriteAllText(Application.dataPath + "/Resources/gameSceneArgs.json", gameSceneArgsJson.ToString());
        }
    }
}
