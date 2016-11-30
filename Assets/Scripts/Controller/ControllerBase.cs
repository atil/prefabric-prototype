﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    // Seems like an unnecessary level in inheritence
    // but I felt I should be explicitly identifying commands
    public abstract class PfCommand : PfEvent { }

    public class MoveCommand : PfCommand
    {
        public Vector3 Direction { get; set; }
    }

    public class CameraRotateCommand : PfCommand
    {
        public Vector2 Amount { get; set; }
    }

    public class CameraZoomCommand : PfCommand
    {
        public float Amount { get; set; }
    }

    /// <summary>
    /// Controllers are command emitters.
    /// These input commands can come from keyboard - mouse,
    /// gamepad, touch, VR hand things and (bear with me),
    /// deserialized byte stream (recorded demo), A.I, network input etc.
    /// </summary>
    public abstract class ControllerBase
    {
        public virtual void Update() { }
    }
}
