using System;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SimpleJSON;
using UnityEngine.SceneManagement;

namespace Prefabric
{
    public class ReturnToMenuEvent : PfSceneEvent { }

    // This might be a command
    public class QuitGameEvent : PfSceneEvent { }

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

        private bool _isLevelTransition; // Let's not go uglier than that okay?

        void Start()
        {
            Music.FadeIn();

            var args = GameSceneArgs.Load();

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
                Music.FadeOut();

                // Wait for UI flash
                // ... but a little less in order not to see a frame of non flashy screen

                Util.WaitForSeconds(Curve.Instance.LevelPassFade.length - 0.1f, () =>
                {
                    if (args.IsEditMode) // Passing level in editor scene
                    {
                        // Load the same level with the same args
                        PfScene.Load("GameScene");
                        return;
                    }

                    // Don't trigger it twice
                    if (_isLevelTransition)
                    {
                        return;
                    }
                    _isLevelTransition = true;

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
                        // Endgame
                        PfScene.Load("MainMenuScene");
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

            MessageBus.OnEvent<ReturnToMenuEvent>().Subscribe(ev =>
            {
                Music.FadeOut();

                Util.WaitForSeconds(Curve.Instance.LevelPassFade.length - 0.1f, () =>
                {
                    PfScene.Load("MainMenuScene");
                });
            });

            MessageBus.OnEvent<QuitGameEvent>().Subscribe(ev =>
            {
                Application.Quit();
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
