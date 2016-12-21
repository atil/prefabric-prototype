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

        public void Init()
        {
            MenuButton.onClick.AddListener(() =>
            {
                EditorMenu.Toggle();
            });

            EditorTileSelector.LoadTileButtons(new[]
            {
                PfResourceType.WhiteTile,
                PfResourceType.GreyTile,
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

            MessageBus.OnEvent<EditorSaveLevelEvent>().Subscribe(ev =>
            {
                MenuButton.interactable = false;
                EditorMenu.SetState(false);
            });

            MessageBus.OnEvent<EditorLoadLevelEvent>().Subscribe(ev =>
            {
                MenuButton.interactable = false;
                EditorMenu.SetState(false);
            });
        }
    }
}
