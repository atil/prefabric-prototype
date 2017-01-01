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
            Flash(WhiteScreen, Curve.Instance.LevelBeginFade);

            // When player is falling down, flash the screen in a nice way
            MessageBus.OnEvent<PlayerFallEvent>().Subscribe(ev =>
            {
                Flash(WhiteScreen, Curve.Instance.PlayerFallFade);
            });

            // Flash on level finish
            MessageBus.OnEvent<EndZoneTriggeredEvent>().Subscribe(ev =>
            {
                Flash(WhiteScreen, Curve.Instance.LevelPassFade);
            });
        }

        private void Flash(Image img, AnimationCurve curve)
        {
            var t = 0f;
            IDisposable fadeDisposable = null;
            fadeDisposable = Observable.EveryUpdate().Subscribe(x =>
            {
                if (img == null) // Sometimes it gets destroyed during a level load
                {
                    return;
                }

                img.SetAlpha(curve.Evaluate(t));

                t += PfTime.DeltaTime;
                if (t > Curve.Instance.LevelPassFade.length)
                {
                    img.SetAlpha(0f);
                    fadeDisposable.Dispose();
                }
            });
        }
    }
}
