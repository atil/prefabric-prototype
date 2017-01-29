using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prefabric;
using System;
using UniRx;

namespace Prefabric.LevelEditor
{
    public class LevelEditorWorld : MonoBehaviour
    {
        private const int ClearSize = 3;

        [SerializeField]
        private EditorCamera _camera;

        [SerializeField]
        private EditorUi _ui;

        private LevelLoader _levelLoader;
        private List<Tile> _tiles;
        private PfResourceType _curResource = PfResourceType.WhiteTile;

        void Start()
        {
            _camera.Init();
            _ui.Init();

            _levelLoader = new LevelLoader();
            _tiles = _levelLoader.LoadLevelAt("testLevel.json");

            _camera.HoverEnter += OnCameraHoverEnter;
            _camera.HoverExit += OnCameraHoverExit;
            _camera.LeftClick += OnCameraLeftClick;
            _camera.RightClick += OnCameraRightClick;

            MessageBus.OnEvent<EditorSaveLevelEvent>().Subscribe(ev =>
            {
                if (!string.IsNullOrEmpty(ev.Path))
                {
                    _levelLoader.SaveLevelAt(_tiles, ev.Path);
                }
            });

            MessageBus.OnEvent<EditorLoadLevelEvent>().Subscribe(ev =>
            {
                if (string.IsNullOrEmpty(ev.Path))
                {
                    return;
                }

                foreach(var tile in _tiles)
                {
                    DestroyImmediate(tile.gameObject);
                }
                _tiles.Clear();

                _tiles = _levelLoader.LoadLevelAt(ev.Path);
            });

            MessageBus.OnEvent<EditorTileSelectedEvent>().Subscribe(ev =>
            {
                _curResource = ev.SelectedTileResource;
            });

            MessageBus.OnEvent<EditorTestLevelEvent>().Subscribe(ev =>
            {
                GameSceneArgs.Write("testLevel.json", true);
                var lvlPath = Application.dataPath + "/Levels/testLevel.json";
                var succ = _levelLoader.SaveLevelAt(_tiles, lvlPath);

                if (succ)
                {
                    PfScene.Load("GameScene");
                }
            });

            MessageBus.OnEvent<EditorClearLevelEvent>().Subscribe(ev =>
            {
                foreach (var tile in _tiles)
                {
                    Destroy(tile.gameObject);
                }
                _tiles.Clear();

                _curResource = PfResourceType.StartTile;
                InstantiateTile(Vector3.zero);

                _curResource = PfResourceType.WhiteTile;
                for (var i = -ClearSize; i < ClearSize; i++)
                {
                    for (var j = -ClearSize; j < ClearSize; j++)
                    {
                        if (i != 0 || j != 0)
                        {
                            InstantiateTile(new Vector3(i, 0, j));
                        }
                    }
                }
            });
        }

        void Update()
        {
            _camera.ExternalUpdate();
        }

        #region Camera Events
        private void OnCameraHoverExit(Tile tile)
        {
            // TODO
        }

        private void OnCameraHoverEnter(Tile tile, Vector3 normal)
        {
            // TODO
        }

        private void OnCameraLeftClick(Tile hitTile, Vector3 normal)
        {
            InstantiateTile(hitTile.Position + normal);
        }

        private void OnCameraRightClick(Tile tile, Vector3 normal)
        {
            _tiles.Remove(tile);
            Destroy(tile.gameObject);
        }
        #endregion

        private void InstantiateTile(Vector3 position)
        {
            var tileGo = Instantiate(PfResources.Load<GameObject>(_curResource), position, Quaternion.identity);
            var newTile = tileGo.GetComponent<Tile>();
            newTile.Init(Guid.NewGuid(), _curResource);
            _tiles.Add(newTile);
        }
    }
}
