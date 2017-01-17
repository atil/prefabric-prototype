using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UnityEngine;

namespace Prefabric
{
    /// <summary>
    /// </summary>
    /// <remarks>I couldn't ever come up with a decent solution to this</remarks>
    public static class Music
    {
        private static readonly AudioClip[] Clips;
        private static readonly AudioSource AudioSource;
        private static readonly AnimationCurve LinearUpCurve;
        private static readonly AnimationCurve LinearDownCurve;
        private const float MusicMaxLevel = 0.4f; // TODO: These should change with some volume option

        static Music()
        {
            Clips = new []
            {
                PfResources.Load<AudioClip>(PfResourceType.SfxMusic1),
                PfResources.Load<AudioClip>(PfResourceType.SfxMusic2),
                PfResources.Load<AudioClip>(PfResourceType.SfxMusic3),
            };

            AudioSource = new GameObject("Music").AddComponent<AudioSource>();
            GameObject.DontDestroyOnLoad(AudioSource.gameObject);
            LinearUpCurve = AnimationCurve.Linear(0, 0, 1, MusicMaxLevel); 
            LinearDownCurve = AnimationCurve.Linear(0, MusicMaxLevel, 1, 0); 
        }

        public static void Play(bool startNewRandom)
        {
            FadeOut();
            Observable.Timer(TimeSpan.FromSeconds(LinearDownCurve.length)).Subscribe(x =>
            {
                FadeIn();

                if (startNewRandom)
                {
                    // Randomize clip
                    AudioSource.clip = Clips.Random();

                    // Tail a new song at the end of it
                    // (not sure if it'll work well)
                    IDisposable musicDisposable = null;
                    musicDisposable = Observable.Timer(TimeSpan.FromSeconds(AudioSource.clip.length)).Subscribe(y =>
                    {
                        Play(true);
                        musicDisposable.Dispose();
                    });
                }

                AudioSource.Play();
            });
        }

        public static void FadeIn()
        {
            var t = 0f;
            IDisposable fadeDisposable = null;
            fadeDisposable = Observable.EveryUpdate().Subscribe(x =>
            {
                AudioSource.volume = LinearUpCurve.Evaluate(t);

                t += Time.unscaledDeltaTime; // Music fadeouts are done with real time
                if (t > LinearUpCurve.length)
                {
                    fadeDisposable.Dispose();
                }
            });

        }

        public static void FadeOut()
        {
            var t = 0f;
            IDisposable fadeDisposable = null;
            fadeDisposable = Observable.EveryUpdate().Subscribe(x =>
            {
                AudioSource.volume = LinearDownCurve.Evaluate(t);

                t += Time.unscaledDeltaTime; // Music fadeouts are done with real time
                if (t > LinearDownCurve.length)
                {
                    fadeDisposable.Dispose();
                }
            });
        }
    }
}
