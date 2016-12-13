using UnityEngine;
using UniRx;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Prefabric
{
    /// <summary>
    /// This is the main MonoBehaviour of GameScene
    /// This Start() and Update() functions are what keeps the game running
    /// </summary>
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
	        var args = GameSceneArgs.Load();

            _keyboardMouseController = new KeyboardMouseController();
            _player = new PlayerAgent(_playerTransform, _camTransform);
            _agents = new List<AgentBase> { _player };
            _mapManager = new MapManager(args.LevelName, _agents);

            MessageBus.OnEvent<EndZoneTriggeredEvent>().Subscribe(x =>
            {
                if (args.IsEditMode) // Passing level in editor scene
                {
                    // Load the same level with the same args
                    PfScene.Load("GameScene");
                    return;
                }

                // Advance level
                var curLevelIndex = LevelPaths.Paths.IndexOf(args.LevelName);
                if (++curLevelIndex >= LevelPaths.Paths.Count)
                {
                    // TODO: All levels done, endgame
                }
                else
                {
                    GameSceneArgs.Write(LevelPaths.Paths[curLevelIndex], false);
                    PfScene.Load("GameScene");
                }
            });

	        MessageBus.OnEvent<MenuToggleCommand>().Subscribe(ev =>
	        {
	            if (args.IsEditMode)
	            {
	                PfScene.Load("LevelEditorScene");
	            }
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
                PfScene.Load("GameScene");
            }
        }
    }
}
