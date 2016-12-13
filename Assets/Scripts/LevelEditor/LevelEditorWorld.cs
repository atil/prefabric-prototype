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
            _tiles = _levelLoader.LoadLevelAt("Levels/testLevel.json");

            _camera.HoverEnter += OnCameraHoverEnter;
            _camera.HoverExit += OnCameraHoverExit;
            _camera.LeftClick += OnCameraLeftClick;
            _camera.RightClick += OnCameraRightClick;

            MessageBus.OnEvent<EditorSaveLevelEvent>().Subscribe(ev =>
            {
                _levelLoader.SaveLevelAt(_tiles, ev.Path);
            });

            MessageBus.OnEvent<EditorLoadLevelEvent>().Subscribe(ev =>
            {
                foreach(var tile in _tiles)
                {
                    DestroyImmediate(tile.gameObject);
                }
                _tiles.Clear();

                if (!string.IsNullOrEmpty(ev.Path))
                {
                    _tiles = _levelLoader.LoadLevelAt(ev.Path);
                }
            });

            MessageBus.OnEvent<EditorTileSelectedEvent>().Subscribe(ev =>
            {
                _curResource = ev.SelectedTileResource;
            });

            MessageBus.OnEvent<EditorTestLevelEvent>().Subscribe(ev =>
            {
                GameSceneArgs.Write("testLevel.json");
                var lvlPath = Application.dataPath + "/Resources/Levels/testLevel.json";
                _levelLoader.SaveLevelAt(_tiles, lvlPath);

                Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(x =>
                {
                    PfScene.Load("GameScene");
                });
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
            var tileGo = Instantiate(PfResources.Load<GameObject>(_curResource));
            var newTile = tileGo.GetComponent<Tile>();
            newTile.Init(Guid.NewGuid());
            newTile.Position = hitTile.Position + normal;
            _tiles.Add(newTile);
        }

        private void OnCameraRightClick(Tile tile, Vector3 normal)
        {
            _tiles.Remove(tile);
            Destroy(tile.gameObject);
        }
        #endregion

    }
}
