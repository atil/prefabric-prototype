using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PrefabricEditor
{
    public class EditorUi : MonoBehaviour
    {
        public Action<bool> MenuToggle;
        public Action<string> LevelSave;
        public Action<string> LevelLoad;

        public Image Crosshair;
        public Button MenuButton;
        public EditorMenu EditorMenu;

        void Start()
        {
            MenuButton.onClick.AddListener(() =>
            {
                EditorMenu.Toggle();
            });
        }
    }
}
