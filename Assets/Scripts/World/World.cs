using System;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SimpleJSON;
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
        private Ui _ui;

        private List<AgentBase> _agents;
        private KeyboardMouseController _keyboardMouseController;
        private MapManager _mapManager;
        private PlayerAgent _player;

        private readonly List<string> _levelPaths = new List<string>();

	    void Start()
	    {
	        var args = GameSceneArgs.Load();

            var tmpList = new List<string>();
            var json = JSON.Parse(PfResources.LoadStringAt("levelPaths.json"));
            foreach (JSONNode pathString in json["paths"].AsArray)
            {
                _levelPaths.Add(pathString.Value);
            }

            _keyboardMouseController = new KeyboardMouseController();

	        var playerGo = Instantiate(PfResources.Load<GameObject>(PfResourceType.Player));

            _player = new PlayerAgent(playerGo.transform, _camTransform);
            _agents = new List<AgentBase> { _player };
            _mapManager = new MapManager(args.LevelName, _agents);
            _ui.Init();

            MessageBus.OnEvent<EndZoneTriggeredEvent>().Subscribe(ev =>
            {
                // Wait for UI flash
                // ... but a little less in order not to see a frame of non flashy screen
                Observable.Timer(TimeSpan.FromSeconds(Curve.Instance.LevelPassFade.length - 0.1)).Subscribe(x =>
                {
                    if (args.IsEditMode) // Passing level in editor scene
                    {
                        // Load the same level with the same args
                        PfScene.Load("GameScene");
                        return;
                    }

                    // Advance level
                    var curLevelIndex = _levelPaths.IndexOf(args.LevelName);

                    if (curLevelIndex == -1)
                    {
                        Debug.LogError("Level not found in levelPaths : " + args.LevelName);
                        return;
                    }

                    curLevelIndex++;
                    if (curLevelIndex >= _levelPaths.Count)
                    {
                        // TODO: All levels done, endgame
                    }
                    else
                    {
                        GameSceneArgs.Write(_levelPaths[curLevelIndex], false);
                        PfScene.Load("GameScene");
                    }
                });
            });

	        MessageBus.OnEvent<MenuToggleCommand>().Subscribe(ev =>
	        {
	            if (args.IsEditMode)
	            {
	                PfScene.Load("LevelEditorScene");
	            }
	            else
	            {
	                _ui.OnMenuToggled();
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
