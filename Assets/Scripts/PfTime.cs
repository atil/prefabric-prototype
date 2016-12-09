using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    /// <summary>
    /// A wrapper around Unity's Time class
    /// Useful when a "timescale = 0f"-like behaviour is needed, 
    /// but also is some of unscaled functionality
    /// Yes, there's Time.unscaledDeltaTime, but that's not enough in some cases
    /// </summary>
    public class PfTime : MonoBehaviour
    {
        public static float Time { get; set; }

        public static float TimeScale { get; set; }

        public static float DeltaTime
        {
            get { return UnityEngine.Time.deltaTime * TimeScale; }
        }

        void Awake()
        {
            TimeScale = 1f;
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            Time += DeltaTime;
        }
    }
}
