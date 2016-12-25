using Prefabric;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Prefabric.LevelEditor
{
    public class EditorCameraStateChangedEvent : PfSceneEvent
    {
        public bool IsActive { get; set; }
    }

    public class EditorCamera : MonoBehaviour
    {
        // TODO ATIL: Convert these to PfEvents
        public Action<Tile, Vector3> Hover;
        public Action<Tile, Vector3> LeftClick;
        public Action<Tile, Vector3> RightClick;
        public Action<Tile, Vector3> HoverEnter;
        public Action<Tile> HoverExit;

        private const float MoveSpeed = 20f;
        private const float RotSpeed = 100f;

        private Tile _currentHoverTile;
        private Transform _tr;
        private Vector3 _midScreenPoint;
        private bool _controlsEnabled;

        public void Init()
        {
            _tr = transform;
            _midScreenPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            SetActive(true);

            MessageBus.OnEvent<EditorSaveLevelEvent>().Subscribe(x =>
            {
                SetActive(true);
            });

            MessageBus.OnEvent<EditorLoadLevelEvent>().Subscribe(x =>
            {
                SetActive(true);
            });
        }

        private void SetActive(bool value)
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !value;
            _controlsEnabled = value;

            MessageBus.Publish(new EditorCameraStateChangedEvent()
            {
                IsActive = value
            });
        }

        public void ExternalUpdate()
        {
            // Cursor enabling
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetActive(!_controlsEnabled);
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
            if (Physics.Raycast(Camera.main.ScreenPointToRay(_midScreenPoint), out hit, float.MaxValue))
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
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(_midScreenPoint), out hit, float.MaxValue))
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
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(_midScreenPoint), out hit, float.MaxValue))
                    {
                        var tile = hit.transform.GetComponent<Tile>();
                        if (tile != null)
                        {
                            RightClick(tile, hit.normal);
                        }
                    }
                }
            }
            
            // Reset camera
            if (Input.GetKeyDown(KeyCode.R))
            {
                _tr.position = Vector3.forward * -10;
                _tr.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            }

        }

    }
}