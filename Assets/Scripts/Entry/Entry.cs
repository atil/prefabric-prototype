using System.Collections;
using System.Collections.Generic;
using UniRx;
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
            Music.Play(true);
            Observable.NextFrame().Subscribe(x =>
            {
                PfScene.Load("MainMenuScene");
            });
        }
       
    }
}
