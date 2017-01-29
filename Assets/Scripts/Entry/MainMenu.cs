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

        public GameObject CreditsPanel;
        public PfButton CreditsButton;
        public PfButton CreditsCloseButton;

        void Start()
        {
            Ui.Flash(WhiteScreen, Curve.Instance.LevelBeginFade);

            PlayButton.Clicked += () =>
            {
                GameSceneArgs.Write("singleBend.json", false); // Load first level
                Ui.Flash(WhiteScreen, Curve.Instance.LevelPassFade);
                Music.Play(true);

                Util.WaitForSeconds(Curve.Instance.LevelPassFade.length, () =>
                {
                    PfScene.Load("GameScene");
                });
            };

            LevelEditorButton.Clicked += () =>
            {
                Music.FadeOut();
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

            CreditsButton.Clicked += () =>
            {
                CreditsPanel.SetActive(true);
            };

            CreditsCloseButton.Clicked += () =>
            {
                CreditsPanel.SetActive(false);
            };
        }
    }
}
