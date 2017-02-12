using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    public class Curve : MonoBehaviour
    {
        private static Curve _instance;

        public static Curve Instance
        {
            get { return _instance ?? (_instance = FindObjectOfType<Curve>()); }
        }

        [Header("Tile Effects")]
        public AnimationCurve TileTween;
        public AnimationCurve TileTweenFade;
        public AnimationCurve TileTweenFadeIn;

        [Header("UI Effects")]
        public AnimationCurve PlayerFallFade;
        public AnimationCurve LevelPassFade;
        public AnimationCurve LevelBeginFade;


    }
}
