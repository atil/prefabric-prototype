using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prefabric
{
    public class Entry : MonoBehaviour
    {
        void Start()
        {
            PfScene.Load("GameScene");
        }
       
    }
}
