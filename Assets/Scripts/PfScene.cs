using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prefabric
{
    public static class PfScene
    {
        public static void Load(string sceneName)
        {
            MessageBus.ClearSceneEvents();
            SceneManager.LoadScene(sceneName);
        }
    }

}