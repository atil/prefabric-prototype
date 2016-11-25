using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PrefabricEditor
{ 
    public class EditorMenu : MonoBehaviour
    {
        public Button LoadButton;
        public Button SaveButton;

        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}