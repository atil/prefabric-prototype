using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    public class KeyboardMouseController : ControllerBase
    {
        private Vector2 _prevMousePos;

        public override void Update()
        {
            base.Update();

            // Agent move
            var dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                dir += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                dir += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A))
            {
                dir += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                dir += Vector3.right;
            }
            Move(dir);

            // Camera move / rotate
            if (Input.GetMouseButton(2))
            {
                CamMove((Vector2)Input.mousePosition - _prevMousePos);
            }
            else if (Input.GetMouseButton(1))
            {
                CamRotate((Vector2)Input.mousePosition - _prevMousePos);
            }
            _prevMousePos = Input.mousePosition;

            // Camera zoom
            CamZoom(Input.GetAxis("Mouse ScrollWheel"));
        }
    }
}
