using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    public class LevelLoader
    {
        private static List<string> _levelPaths = new List<string>();

        public LevelLoader()
        {
            var json = JSON.Parse(PfResources.LoadStringAt("levelPaths"));
            foreach (JSONNode pathString in json["paths"].AsArray)
            {
                _levelPaths.Add(pathString.Value);
            }
        }

        public List<Tile> LoadLevelAt(int lvlNum)
        {
            return ParseLevel(PfResources.LoadStringAt(_levelPaths[lvlNum]));
        }

        public List<Tile> LoadLevelAt(string lvlPath)
        {
            return ParseLevel(PfResources.LoadStringAt(lvlPath));
        }

        private List<Tile> ParseLevel(string lvlStr)
        {
            var whiteTilePrefab = PfResources.Load<GameObject>(PfResourceType.WhiteTile);

            var lvlJson = JSON.Parse(lvlStr);
            var tiles = new List<Tile>();
            foreach (JSONClass tileEntry in lvlJson["tiles"].AsArray)
            {
                var tilePos = new Vector3(tileEntry["x"].AsInt, tileEntry["y"].AsInt, tileEntry["z"].AsInt);
                var tileGo = UnityEngine.Object.Instantiate(whiteTilePrefab, tilePos, Quaternion.identity) as GameObject;

                tiles.Add(tileGo.GetComponent<Tile>());
            }
            return tiles;
        }

        public bool SaveLevelAt(List<Tile> tiles, string lvlPath)
        {
            var overwritten = false;
            if (System.IO.File.Exists(lvlPath))
            {
                overwritten = true;
            }

            var lvlJson = new JSONClass();
            lvlJson.Add("tiles", new JSONArray());
            var tilesNode = lvlJson["tiles"];

            for (var i = 0; i < tiles.Count; i++)
            {
                var tile = tiles[i];
                tilesNode[i]["x"].AsInt = Mathf.FloorToInt(tile.Position.x);
                tilesNode[i]["y"].AsInt = Mathf.FloorToInt(tile.Position.y);
                tilesNode[i]["z"].AsInt = Mathf.FloorToInt(tile.Position.z);
            }

            System.IO.File.WriteAllText(lvlPath, lvlJson.ToString());

            return overwritten;
        }

    }
}
