using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabric.LevelEditor
{
    public class EditorUi : MonoBehaviour
    {
        public Action<bool> MenuToggle;

        public Image Crosshair;
        public Button MenuButton;
        public EditorMenu EditorMenu;
        public EditorTileSelector EditorTileSelector;

        void Start()
        {
            MenuButton.onClick.AddListener(() =>
            {
                EditorMenu.Toggle();
            });

            EditorTileSelector.LoadTileButtons(new[]
            {
                PfResourceType.WhiteTile,
                PfResourceType.StartTile,
                PfResourceType.EndTile,
            });

            MessageBus.OnEvent<EditorCameraStateChangedEvent>().Subscribe(ev =>
            {
                MenuButton.interactable = !ev.IsActive;

                if (ev.IsActive)
                {
                    EditorMenu.SetState(false);
                }
            });
        }
    }
}
