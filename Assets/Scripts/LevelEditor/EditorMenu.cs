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

    public class EditorTestLevelEvent : PfSceneEvent
    {
    }

    public class EditorMenu : MonoBehaviour
    {
        public Button TestLevelButton;
        public Button LoadButton;
        public Button SaveButton;

        void Start()
        {
            SaveButton.onClick.AddListener(() =>
            {
                // Temporarily bound to UnityEditor
                var path = UnityEditor.EditorUtility
                    .SaveFilePanel("Save Level", "Assets/Resources/Levels", "UntitledLevel", "json");

                MessageBus.Publish(new EditorSaveLevelEvent() { Path = path });
            });

            LoadButton.onClick.AddListener(() =>
            {
                var path = UnityEditor.EditorUtility
                    .OpenFilePanel("Load Level", Application.dataPath + "/Resources/Levels/", "json");

                path = path.Split('.')[0]; // Crop extension
                path = path.Replace(Application.dataPath + "/Resources/", ""); // Make the path relative to Resources

                MessageBus.Publish(new EditorLoadLevelEvent() { Path = path });
            });

            TestLevelButton.onClick.AddListener(() =>
            {
                MessageBus.Publish(new EditorTestLevelEvent());
            });

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