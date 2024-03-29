﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    /// <summary>
    /// Emits commands from WASD-mouse controls
    /// </summary>
    public class KeyboardMouseController : ControllerBase
    {
        private Vector2 _prevMousePos;
        private float _doubleClickTimer;

        public override void Update()
        {
            base.Update();

            // Menu toggle
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                MessageBus.Publish(new MenuToggleCommand());
                IsEnabled = !IsEnabled;
            }

            // Toggling menu locks this controller
            if (!IsEnabled)
            {
                return;
            }

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
            MessageBus.Publish(new MoveCommand() { Direction = dir });

            // Tile selecting / hovering on
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, 1 << Layer.Tile))
            {
                var hitTile = hit.transform.GetComponent<Tile>();
                if (!(hitTile is StartTile) && !(hitTile is EndTile))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        MessageBus.Publish(new TileSelectCommand() { Tile = hitTile });
                    }
                    else
                    {
                        MessageBus.Publish(new TileHoverCommand() { Tile = hitTile });
                    }
                }
            }

            // Click on nothing deselects tile
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue)
                && Input.GetMouseButton(0))
            {
                MessageBus.Publish(new TileDeselectCommand());
            }
            
            // Camera rotate
            if (Input.GetMouseButton(1))
            {
                MessageBus.Publish(new CameraRotateCommand() { Amount = (Vector2)Input.mousePosition - _prevMousePos });
            }
            _prevMousePos = Input.mousePosition;

            // Double click
            if (Input.GetMouseButtonDown(1))
            {
                if (_doubleClickTimer < 0.3f)
                {
                    MessageBus.Publish(new UnbendCommand());
                }

                _doubleClickTimer = 0;
            }
            _doubleClickTimer += Time.deltaTime;

            // Camera zoom
            MessageBus.Publish(new CameraZoomCommand() { Amount = Input.GetAxis("Mouse ScrollWheel") });
        }
    }
}
