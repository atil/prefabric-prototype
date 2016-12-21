using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UnityEngine;

namespace Prefabric
{
    /// <summary>
    /// Saves tile list to specified file and reads tiles from file
    /// </summary>
    public class LevelLoader
    {
        /// <summary>
        /// Load level at the specified path
        /// </summary>
        /// <param name="lvlPath"></param>
        /// <returns>A list of tiles</returns>
        public List<Tile> LoadLevelAt(string lvlPath)
        {
            return ParseLevel(PfResources.LoadStringAt("/Levels/" + lvlPath)); ;
        }

        /// <summary>
        /// Actual parsing of json file
        /// Loaded level is guaranteed to have exactly one start and one end tile
        /// </summary>
        /// <param name="lvlStr">Json string</param>
        /// <returns></returns>
        private List<Tile> ParseLevel(string lvlStr)
        {
            var lvlJson = JSON.Parse(lvlStr);
            var tiles = new List<Tile>();

            foreach (JSONClass tileEntry in lvlJson["tiles"].AsArray)
            {
                var tileId = tileEntry["id"].Value;
                var tilePos = new Vector3(tileEntry["x"].AsInt, tileEntry["y"].AsInt, tileEntry["z"].AsInt);
                var tileType = tileEntry["resourceType"].Value.ToEnum<PfResourceType>();
                var tilePrefab = PfResources.Load<GameObject>(tileType); // TODO: This can be cached
                var tileGo = UnityEngine.Object.Instantiate(tilePrefab, tilePos, Quaternion.identity) as GameObject;
                var tile = tileGo.GetComponent<Tile>();
                tile.Init(new Guid(tileId), tileType);
                tiles.Add(tileGo.GetComponent<Tile>());
            }

            return tiles;
        }

        /// <summary>
        /// Write a tile list to a path, in JSON format.
        /// Start and end tiles are written to the end
        /// </summary>
        /// <param name="tiles"></param>
        /// <param name="lvlPath"></param>
        /// <returns>False, if path is errorneous or start/end tiles are problematic. Otherwise true</returns>
        public bool SaveLevelAt(List<Tile> tiles, string lvlPath)
        {
            if (string.IsNullOrEmpty(lvlPath))
            {
                return false;
            }

            if (tiles.FindAll(t => t is StartTile).Count != 1 
                || tiles.FindAll(t => t is EndTile).Count != 1)
            {
                Debug.LogError("Couldn't save level. It has to contain exactly one start and one end tile : " + lvlPath);
                return false;
            }

            var lvlJson = new JSONClass();
            lvlJson.Add("tiles", new JSONArray());

            var tilesNode = lvlJson["tiles"];
            var i = 0;
            foreach(var tile in tiles)
            {
                tilesNode[i]["id"] = tile.Id.ToString();
                tilesNode[i]["resourceType"] = tile.ResourceType.ToString();
                tilesNode[i]["x"].AsInt = Mathf.FloorToInt(tile.Position.x);
                tilesNode[i]["y"].AsInt = Mathf.FloorToInt(tile.Position.y);
                tilesNode[i]["z"].AsInt = Mathf.FloorToInt(tile.Position.z);
                i++;
            }

            System.IO.File.WriteAllText(lvlPath, lvlJson.ToString());

            Debug.Log("Level written to : [" + lvlPath + "] Tile count: " + tiles.Count);

            return true;
        }

    }
}
