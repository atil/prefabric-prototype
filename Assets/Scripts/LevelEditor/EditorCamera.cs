using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrefabricEditor
{
    public class EditorCamera : MonoBehaviour
    {
        public Action<Vector3, Vector3> Hover;
        public Action<Vector3, Vector3> Click;

        private Transform _tr;
        private const float MoveSpeed = 5f;
        private const float RotSpeed = 50f;

        void Start()
        {
            _tr = transform;
        }

        public void SetEnabled(bool isEnabled)
        {
            enabled = isEnabled;
        }

        void Update()
        {
            // Move
            var moveDir = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                moveDir += _tr.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDir -= _tr.forward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveDir -= _tr.right;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveDir += _tr.right;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                moveDir += Vector3.up;
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                moveDir += Vector3.down;
            }

            _tr.Translate(moveDir.normalized * MoveSpeed * Time.deltaTime, Space.World);

            // Look
            _tr.Rotate(Vector3.up * Input.GetAxis("Mouse X") * RotSpeed * Time.deltaTime, Space.World);
            _tr.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * RotSpeed * Time.deltaTime, Space.Self);
        }

    }
}