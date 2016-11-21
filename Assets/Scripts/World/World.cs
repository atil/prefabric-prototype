using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Prefabric
{
    public class World : MonoBehaviour
    {
        [SerializeField]
        private Transform _playerTransform;

        private KeyboardMouseController _keyboardMouseController;
        private MapManager _mapManager;
        private PlayerAgent _player;

	    void Start () 
	    {
            _keyboardMouseController = new KeyboardMouseController();
            _player = new PlayerAgent(_playerTransform, _keyboardMouseController);
            _mapManager = new MapManager(0, _keyboardMouseController, new List<AgentBase> { _player });
	    }
    }
}
