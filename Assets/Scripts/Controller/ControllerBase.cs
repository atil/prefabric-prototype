using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    /// <summary>
    /// Controllers are command emitters.
    /// These input commands can come from keyboard - mouse,
    /// gamepad, touch, VR hand things and (bear with me),
    /// deserialized byte stream (recorded demo), A.I, network input etc.
    /// </summary>
    public abstract class ControllerBase
    {
        /// <summary>
        /// Where the commands come from, most likely they are going to move agents
        /// </summary>
        public Action<Vector3> Move;

        /// <summary>
        /// Camera movement commands...
        /// </summary>
        public Action<Vector2> CamMove;
        public Action<Vector2> CamRotate;
        public Action<float> CamZoom;

        public virtual void Update() { }
    }
}
