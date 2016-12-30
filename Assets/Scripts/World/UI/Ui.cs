using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabric
{
    /// <summary>
    /// GameScene user interface
    /// Kept MonoBheaviour because we'd like to drag'n'drop UI elements
    /// That could be done in World.cs, but there'd be lotta cluttering there
    /// </summary>
    public class Ui : MonoBehaviour
    {
        public Image WhiteScreen;

        public void Init()
        {
            // When player is falling down, flash the screen in a nice way
            MessageBus.OnEvent<PlayerFallEvent>().Subscribe(ev =>
            {
                var t = 0f;
                IDisposable fadeDisposable = null;
                fadeDisposable = Observable.EveryUpdate().Subscribe(x =>
                {
                    WhiteScreen.SetAlpha(Curve.Instance.WhiteScreenFade.Evaluate(t));

                    t += PfTime.DeltaTime;
                    if (t > 1)
                    {
                        WhiteScreen.SetAlpha(0f);
                        fadeDisposable.Dispose();
                    }
                });
            });
        }
    }
}
