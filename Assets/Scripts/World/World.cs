using UnityEngine;
using System.Collections;
using UniRx;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Prefabric
{
    public class World : MonoBehaviour
    {
        [SerializeField]
        private Transform _camTransform;

        [SerializeField]
        private Transform _playerTransform;

        private List<AgentBase> _agents;
        private KeyboardMouseController _keyboardMouseController;
        private MapManager _mapManager;
        private PlayerAgent _player;

	    void Start() 
	    {
            _keyboardMouseController = new KeyboardMouseController(_camTransform);
            _player = new PlayerAgent(_playerTransform, _keyboardMouseController);
            _agents = new List<AgentBase> { _player };
            _mapManager = new MapManager(0, _keyboardMouseController, _agents);

            MessageBus.OnEvent<EndZoneTriggeredEvent>().Subscribe(x =>
            {
                // Advance level counter

                SceneManager.LoadScene("GameScene");
            });
	    }

        void Update()
        {
            // Emit commands
            _keyboardMouseController.Update();

            // Let agents do whatever they want with those commands
            foreach (var agent in _agents)
            {
                agent.Update();
            }

            _mapManager.Update();

            // To make testing easier
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("GameScene");
            }
        }
    }
}
