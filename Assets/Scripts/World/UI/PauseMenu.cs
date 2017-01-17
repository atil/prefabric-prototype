using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabric
{
    public class PauseMenu : MonoBehaviour
    {
        public Text HeaderText;
        public PfButton ResumeButton;
        public PfButton ReturnToMenuButton;
        public PfButton QuitButton;

        void Start()
        {
            ResumeButton.Clicked += () =>
            {
                MessageBus.Publish(new MenuToggleCommand());
            };

            ReturnToMenuButton.Clicked += () =>
            {
                MessageBus.Publish(new ReturnToMenuEvent());
            };

            QuitButton.Clicked += () =>
            {
                MessageBus.Publish(new QuitGameEvent());
            };
        }
    }
}
