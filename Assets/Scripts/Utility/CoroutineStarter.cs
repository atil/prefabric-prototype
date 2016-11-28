using System.Collections;
using UnityEngine;

namespace Prefabric
{
    public static class CoroutineStarter
    {
        private class CoroutineStarterSlave : MonoBehaviour { }

        private static readonly CoroutineStarterSlave coroutineStarter;
        public static Coroutine StartCoroutine(IEnumerator function)
        {
            return coroutineStarter.StartCoroutine(function);
        }

        public static void StopCoroutine(IEnumerator function)
        {
            if (function != null)
            {
                coroutineStarter.StopCoroutine(function);
            }
        }

        static CoroutineStarter()
        {
            coroutineStarter = new GameObject("CoroutineStarter").AddComponent<CoroutineStarterSlave>();
            Object.DontDestroyOnLoad(coroutineStarter.gameObject);
        }
    }
}