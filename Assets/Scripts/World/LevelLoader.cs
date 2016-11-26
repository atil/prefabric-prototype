using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    /// <summary>
    /// Saves tile list to specified file and reads tiles from file
    /// </summary>
    public class LevelLoader
    {
        /// <summary>
        /// Level paths of single player mode
        /// </summary>
        private static List<string> _levelPaths = new List<string>();

        public LevelLoader()
        {
            var json = JSON.Parse(PfResources.LoadStringAt("levelPaths"));
            foreach (JSONNode pathString in json["paths"].AsArray)
            {
                _levelPaths.Add(pathString.Value);
            }
        }

        /// <summary>
        /// Load level at specific index
        /// </summary>
        /// <param name="lvlNum">Index in single player</param>
        /// <returns></returns>
        public List<Tile> LoadLevelAt(int lvlNum)
        {
            return ParseLevel(PfResources.LoadStringAt(_levelPaths[lvlNum]));
        }

        /// <summary>
        /// Load level at the specified path
        /// </summary>
        /// <param name="lvlPath">A file path relative to Resources</param>
        /// <returns></returns>
        public List<Tile> LoadLevelAt(string lvlPath)
        {
            return ParseLevel(PfResources.LoadStringAt(lvlPath));
        }

        /// <summary>
        /// Actual parsing of json file
        /// Loaded level is guaranteed to have exactly one start and one end tile
        /// </summary>
        /// <param name="lvlStr">Json string</param>
        /// <returns></returns>
        private List<Tile> ParseLevel(string lvlStr)
        {
            // These should be probably somewhere else
            var whiteTilePrefab = PfResources.Load<GameObject>(PfResourceType.WhiteTile);
            var startTilePrefab = PfResources.Load<GameObject>(PfResourceType.StartTile);
            var endTilePrefab = PfResources.Load<GameObject>(PfResourceType.EndTile);

            var lvlJson = JSON.Parse(lvlStr);
            var tiles = new List<Tile>();
            foreach (JSONClass tileEntry in lvlJson["tiles"].AsArray)
            {
                var tilePos = new Vector3(tileEntry["x"].AsInt, tileEntry["y"].AsInt, tileEntry["z"].AsInt);
                var tileGo = UnityEngine.Object.Instantiate(whiteTilePrefab, tilePos, Quaternion.identity) as GameObject;

                // TODO ATIL: Instantiating under a transform
                tiles.Add(tileGo.GetComponent<Tile>());
            }

            var startTilePos = new Vector3(lvlJson["startTile"]["x"].AsInt, 
                lvlJson["startTile"]["y"].AsInt, 
                lvlJson["startTile"]["z"].AsInt);
            var startTileGo = UnityEngine.Object.Instantiate(startTilePrefab, startTilePos, Quaternion.identity) as GameObject;
            tiles.Add(startTileGo.GetComponent<StartTile>());

            var endTilePos = new Vector3(lvlJson["endTile"]["x"].AsInt,
                lvlJson["endTile"]["y"].AsInt,
                lvlJson["endTile"]["z"].AsInt);
            var endTileGo = UnityEngine.Object.Instantiate(endTilePrefab, endTilePos, Quaternion.identity) as GameObject;
            tiles.Add(endTileGo.GetComponent<EndTile>());

            return tiles;
        }

        /// <summary>
        /// Write a tile list to a path, in JSON format.
        /// Start and end tiles are written to the end
        /// </summary>
        /// <param name="tiles"></param>
        /// <param name="lvlPath"></param>
        /// <returns></returns>
        public bool SaveLevelAt(List<Tile> tiles, string lvlPath)
        {
            var startTiles = tiles.FindAll(t => t is StartTile);
            var endTiles = tiles.FindAll(t => t is EndTile);
            if (startTiles.Count != 1 || endTiles.Count != 1)
            {
                Debug.LogError("Couldn't save level. It has to contain exactly one start and one end tile : " + lvlPath);
                return false;
            }

            var startTile = startTiles[0] as StartTile;
            var endTile = endTiles[0] as EndTile;

            var lvlJson = new JSONClass();
            lvlJson.Add("tiles", new JSONArray());

            var tilesNode = lvlJson["tiles"];
            var i = 0;
            for (i = 0; i < tiles.Count; i++)
            {
                var tile = tiles[i];

                // Start/End tiles are not included in "tiles" array
                if (tile is StartTile || tile is EndTile)
                {
                    continue;
                }

                tilesNode[i]["x"].AsInt = Mathf.FloorToInt(tile.Position.x);
                tilesNode[i]["y"].AsInt = Mathf.FloorToInt(tile.Position.y);
                tilesNode[i]["z"].AsInt = Mathf.FloorToInt(tile.Position.z);
            }

            lvlJson["startTile"]["x"].AsInt = Mathf.FloorToInt(startTile.Position.x);
            lvlJson["startTile"]["y"].AsInt = Mathf.FloorToInt(startTile.Position.y);
            lvlJson["startTile"]["z"].AsInt = Mathf.FloorToInt(startTile.Position.z);
            lvlJson["endTile"]["x"].AsInt = Mathf.FloorToInt(endTile.Position.x);
            lvlJson["endTile"]["y"].AsInt = Mathf.FloorToInt(endTile.Position.y);
            lvlJson["endTile"]["z"].AsInt = Mathf.FloorToInt(endTile.Position.z);

            System.IO.File.WriteAllText(lvlPath, lvlJson.ToString());

            return true;
        }

    }
}
