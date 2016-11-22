using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace Prefabric
{
    public class MapManager
    {
        private class Tile
        {
            private GameObject _go;

            public Tile(GameObject go)
            {
                _go = go;
            }
        }
        
        private readonly ControllerBase _controller;
        private readonly List<AgentBase> _agents = new List<AgentBase>();
        private readonly PlayerAgent _player;
        private List<Tile> _tiles;

        public MapManager(int lvlNum, ControllerBase controller, List<AgentBase> agents)
        {
            _controller = controller;
            _agents = agents;
            _player = _agents.Find(x => x is PlayerAgent) as PlayerAgent;

            _tiles = LoadLevel(lvlNum);
        }

        private List<Tile> LoadLevel(int lvlNum)
        {
            var tiles = new List<Tile>();
            var whiteTilePrefab = PfResources.Load<GameObject>(PfResourceType.WhiteTile);

            var lvlJson = JSON.Parse(PfResources.LevelStringOf(lvlNum));
            foreach (JSONClass tileEntry in lvlJson["tiles"].AsArray)
            {
                var tilePos = new Vector3(tileEntry["x"].AsInt, tileEntry["y"].AsInt, tileEntry["z"].AsInt);
                var tileGo = Object.Instantiate(whiteTilePrefab, tilePos, Quaternion.identity) as GameObject;

                tiles.Add(new Tile(tileGo));
            }

            return tiles;
        }
    }
}