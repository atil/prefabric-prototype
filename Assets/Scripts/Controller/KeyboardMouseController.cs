using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    public class KeyboardMouseController : ControllerBase
    {
        private readonly Transform _camTransform;

        public KeyboardMouseController(Transform camTransform) 
        {
            _camTransform = camTransform;
        }

        public override void Update()
        {
            base.Update();

            var dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                dir += _camTransform.forward.Horizontal();
            }
            if (Input.GetKey(KeyCode.S))
            {
                dir += (-_camTransform.forward).Horizontal();
            }
            if (Input.GetKey(KeyCode.A))
            {
                dir += (-_camTransform.right).Horizontal();
            }
            if (Input.GetKey(KeyCode.D))
            {
                dir += _camTransform.right.Horizontal();
            }

            if (dir.sqrMagnitude > 0.001)
            {
                Move(dir);
            }
        }
    }
}
