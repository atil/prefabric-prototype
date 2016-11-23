using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prefabric;
using System;

namespace PrefabricEditor
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

            _ui.MenuToggle += OnMenuToggled;
            _ui.LevelSave += OnLevelSave;
            _ui.LevelLoad += OnLevelLoad;

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

        #region UI Events
        private void OnMenuToggled(bool isOn)
        {
        }

        private void OnLevelSave(string lvlPath)
        {

        }

        private void OnLevelLoad(string lvlPath)
        {

        }
        #endregion
    }
}
