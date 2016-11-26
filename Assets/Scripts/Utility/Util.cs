using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    public static class Layer
    {
        public static LayerMask Tile { get; private set; }

        static Layer()
        {
            Tile = LayerMask.NameToLayer("Tile");
        }

    }

    public static class Util
    {
        public static Vector3 Horizontal(this Vector3 v)
        {
            return Vector3.ProjectOnPlane(v, Vector3.up);
        }
    }
}
