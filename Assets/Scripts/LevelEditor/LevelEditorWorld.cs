using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prefabric;

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

        void Start ()
        {
            _levelLoader = new LevelLoader();
            _tiles = _levelLoader.LoadLevelAt("Levels/testLevel");

            _camera.Hover += OnCameraHover;
            _camera.Click += OnCameraClick;

            _ui.MenuToggle += OnMenuToggled;
            _ui.LevelSave += OnLevelSave;
            _ui.LevelLoad += OnLevelLoad;

        }

        void OnCameraHover(Vector3 pos, Vector3 normal)
        {

        }

        void OnCameraClick(Vector3 pos, Vector3 normal)
        {

        }

        void OnMenuToggled(bool isOn)
        {
            _camera.SetEnabled(!isOn);
        }

        void OnLevelSave(string lvlPath)
        {

        }

        void OnLevelLoad(string lvlPath)
        {

        }
    }
}
