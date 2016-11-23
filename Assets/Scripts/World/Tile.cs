using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    public class Tile : MonoBehaviour
    {
        public Transform Transform { get; private set; }
        public Vector3 Position { get { return Transform.position; } set { Transform.position = value; } }

        public void Init()
        {
            Transform = transform;
        }

        public void ExternalUpdate()
        {

        }

    }
}
