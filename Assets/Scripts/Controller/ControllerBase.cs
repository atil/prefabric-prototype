using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    public abstract class ControllerBase
    {
        public Action<Vector3> Move;

        protected virtual void Update() { }
    }
}
