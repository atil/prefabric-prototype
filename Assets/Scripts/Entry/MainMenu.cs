﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabric
{
    public class MainMenu : MonoBehaviour
    {
        public Image WhiteScreen;
        public Button PlayButton;
        public Button LevelEditorButton;
        public Button GithubButton;
        public Button ExitButton;

        void Start()
        {
            PlayButton.onClick.AddListener(() =>
            {
                GameSceneArgs.Write("singleBend.json", false);
                PfScene.Load("GameScene");
            });

            LevelEditorButton.onClick.AddListener(() =>
            {
                PfScene.Load("LevelEditorScene");
            });

            GithubButton.onClick.AddListener(() =>
            {
                Application.OpenURL("https://github.com/atil/prefabric");
            });

            ExitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
    }
}