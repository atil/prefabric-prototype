using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prefabric
{
    /// <summary>
    /// This scene exists to load any resources that may be 
    /// necessary throughout the whole game
    /// </summary>
    public class Entry : MonoBehaviour
    {
        void Start()
        {
            PfScene.Load("MainMenuScene");
        }
       
    }
}
