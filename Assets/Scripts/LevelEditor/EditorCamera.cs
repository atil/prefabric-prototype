using Prefabric;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace PrefabricEditor
{
    //public class TestEvent : IObservable<TestEvent>
    //{
    //    public bool val;

    //    public IDisposable Subscribe(IObserver<TestEvent> observer)
    //    {

    //    }
    //}

    public class EditorCamera : MonoBehaviour
    {
        public Action<Tile, Vector3> Hover;
        public Action<Tile, Vector3> LeftClick;
        public Action<Tile, Vector3> RightClick;

        public Action<Tile, Vector3> HoverEnter;
        public Action<Tile> HoverExit;
        private Tile _currentHoverTile;

        private Transform _tr;
        private const float MoveSpeed = 5f;
        private const float RotSpeed = 50f;
        private Vector3 _midScreen;
        private bool _controlsEnabled;

        void Start()
        {
            _tr = transform;
            _midScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            _controlsEnabled = true;
            SetCursor(false);
        }

        private void SetCursor(bool isOn)
        {
            Cursor.lockState = isOn ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isOn;
        }

        void Update()
        {
            // Cursor enabling
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetCursor(_controlsEnabled);
                _controlsEnabled = !_controlsEnabled;
            }

            if (!_controlsEnabled)
            {
                return;
            }

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

            // Hover
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(_midScreen), out hit, float.MaxValue))
            {
                var tile = hit.transform.GetComponent<Tile>();
                if (tile != null)
                {
                    if (tile != _currentHoverTile)
                    {
                        HoverEnter(tile, hit.normal);
                        HoverExit(_currentHoverTile);
                        _currentHoverTile = tile;
                    }
                }
            }

            // Left click
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(_midScreen), out hit, float.MaxValue))
                {
                    var tile = hit.transform.GetComponent<Tile>();
                    if (tile != null)
                    {
                        LeftClick(tile, hit.normal);
                    }
                }
            }

            // Right click
            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(_midScreen), out hit, float.MaxValue))
                {
                    var tile = hit.transform.GetComponent<Tile>();
                    if (tile != null)
                    {
                        RightClick(tile, hit.normal);
                    }
                }
            }
        }

    }
}