using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace Prefabric
{
    public class MapManager
    {
        private readonly ControllerBase _controller;
        private readonly List<AgentBase> _agents = new List<AgentBase>();
        private readonly PlayerAgent _player;
        private readonly List<Tile> _tiles;
        private readonly LevelLoader _levelLoader = new LevelLoader();

        public MapManager(int lvlNum, ControllerBase controller, List<AgentBase> agents)
        {
            _controller = controller;
            _agents = agents;
            _player = _agents.Find(x => x is PlayerAgent) as PlayerAgent;

            _tiles = _levelLoader.LoadLevelAt(lvlNum);

            foreach (var tile in _tiles)
            {
                tile.Init();
            }

            _levelLoader.SaveLevelAt(_tiles, Application.dataPath + "/Resources/Levels/testWriteLevel.json");

        }

        public void Update()
        {
            foreach (var tile in _tiles)
            {
                tile.ExternalUpdate();
            }
        }
        
    }
}