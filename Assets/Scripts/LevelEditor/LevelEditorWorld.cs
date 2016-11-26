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

        void Start()
        {
            _camera.Init();

            _levelLoader = new LevelLoader();
            _tiles = _levelLoader.LoadLevelAt("Levels/testLevel");
            foreach (var tile in _tiles)
            {
                tile.Init();
            }

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

                _tiles = _levelLoader.LoadLevelAt(ev.Path);
            });
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
            var tileGo = Instantiate(PfResources.Load<GameObject>(PfResourceType.WhiteTile));
            var newTile = tileGo.GetComponent<Tile>();
            newTile.Init();
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
