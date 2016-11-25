using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabric.LevelEditor
{ 
    public class EditorMenuToggledEvent : PfEvent
    {
        public bool IsActive { get; set; }
    }

    public class EditorMenu : MonoBehaviour
    {
        public Button LoadButton;
        public Button SaveButton;

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