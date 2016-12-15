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

        public AnimationCurve TileTween;
    }
}
