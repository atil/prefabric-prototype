using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabric.LevelEditor
{ 
    public class EditorSaveLevelEvent : PfSceneEvent
    {
        public string Path { get; set; }
    }

    public class EditorLoadLevelEvent : PfSceneEvent
    {
        public string Path { get; set; }
    }

    public class EditorTestLevelEvent : PfSceneEvent { }

    public class EditorClearLevelEvent : PfSceneEvent { }

    public class EditorMenu : MonoBehaviour
    {
        public Button TestLevelButton;
        public Button LoadButton;
        public Button SaveButton;
        public Button ClearButton;

        public EditorSavePanel SavePanel;
        public EditorLoadPanel LoadPanel;

        void Start()
        {
            SaveButton.onClick.AddListener(() =>
            {
                SavePanel.SetActive(true);
                SetState(false);
            });

            LoadButton.onClick.AddListener(() =>
            {
                LoadPanel.SetActive(true);
                SetState(false);
            });

            TestLevelButton.onClick.AddListener(() =>
            {
                MessageBus.Publish(new EditorTestLevelEvent());
            });

            ClearButton.onClick.AddListener(() =>
            {
                MessageBus.Publish(new EditorClearLevelEvent());
            });

            SavePanel.Init();
            LoadPanel.Init();
        }

        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        public void SetState(bool isOn)
        {
            gameObject.SetActive(isOn);
        }
    }
}