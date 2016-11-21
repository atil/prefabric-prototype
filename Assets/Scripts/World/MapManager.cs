using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Prefabric
{
    public class MapManager
    {
        private readonly ControllerBase _controller;
        private readonly List<AgentBase> _agents = new List<AgentBase>();
        private readonly PlayerAgent _player;

        public MapManager(uint lvlNum, ControllerBase controller, List<AgentBase> agents)
        {
            _controller = controller;
            _agents = agents;
            _player = _agents.Find(x => x is PlayerAgent) as PlayerAgent;
        }
    }
}