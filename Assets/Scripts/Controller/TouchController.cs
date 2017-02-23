using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CnControls;
using UnityEngine;

namespace Prefabric
{
    /// <summary>
    /// Emits commands from a touch screen
    /// </summary>
    public class TouchController : ControllerBase
    {
        public override void Update()
        {
            base.Update();

            var dir = new Vector3(CnInputManager.GetAxis("Horizontal"), CnInputManager.GetAxis("Vertical"));
            MessageBus.Publish(new MoveCommand() { Direction = dir });

        }
    }
}
