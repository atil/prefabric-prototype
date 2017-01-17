using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabric
{
    public class MainMenu : MonoBehaviour
    {
        public Image WhiteScreen;
        public PfButton PlayButton;
        public PfButton LevelEditorButton;
        public PfButton GithubButton;
        public PfButton ExitButton;

        void Start()
        {
            Ui.Flash(WhiteScreen, Curve.Instance.LevelBeginFade);

            PlayButton.Clicked += () =>
            {
                GameSceneArgs.Write("singleBend.json", false);
                Ui.Flash(WhiteScreen, Curve.Instance.LevelPassFade);
                Observable.Timer(TimeSpan.FromSeconds(Curve.Instance.LevelPassFade.length)).Subscribe(x =>
                {
                    PfScene.Load("GameScene");
                });
            };

            LevelEditorButton.Clicked += () =>
            {
                PfScene.Load("LevelEditorScene");
            };

            GithubButton.Clicked += () =>
            {
                Application.OpenURL("https://github.com/atil/prefabric");
            };

            ExitButton.Clicked += () =>
            {
                Application.Quit();
            };
        }
    }
}
