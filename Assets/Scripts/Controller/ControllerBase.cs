using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    /// <summary>
    /// Commands are basic things that are told to world and agents
    /// Seems like an unnecessary level in inheritence 
    /// but I felt I should be explicitly identifying commands
    /// </summary>
    public abstract class PfCommand : PfSceneEvent { }

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

    public class TileHoverCommand : PfCommand
    {
        public Tile Tile { get; set; }
    }

    public class TileSelectCommand : PfCommand
    {
        public Tile Tile { get; set; }
    }

    public class UnbendCommand : PfCommand { }

    public class MenuToggleCommand : PfCommand { }

    /// <summary>
    /// Controllers are command emitters.
    /// These input commands can come from keyboard - mouse,
    /// gamepad, touch, VR hand things and (bear with me),
    /// deserialized byte stream (recorded demo), A.I, network input etc.
    /// For more details, visit: http://atilkockar.com/prefabric-devlog-1/
    /// </summary>
    public abstract class ControllerBase
    {
        public virtual void Update() { }
    }
}
