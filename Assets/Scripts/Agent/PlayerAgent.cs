﻿using System;
using UniRx;
using UnityEngine;

namespace Prefabric
{
    public class PlayerFallEvent : PfSceneEvent { }

    public class PlayerAgent : AgentBase
    {
        private const float MaxSpeed = 300f;
        private readonly CharacterController _characterController;

        private const float CamRotateSpeed = 1f;
        private const float CamZoomSpeed = 400f;
        private const float CamMinDistance = 5f;
        private const float CamMaxDistance = 20f;

        /// <summary>
        /// Sometimes we may want to make the camera more snappy
        /// </summary>
        private float _camFollowCoeff = 1f;

        private float _camFollowDistance = 15f;
        private readonly Transform _camTransform;
        private readonly Camera _cam;

        /// <summary>
        /// Command received regarding player's movement direction
        /// </summary>
        private Vector3 _cmdMovDir;

        /// <summary>
        /// Camera will always try to move to this position
        /// </summary>
        private Vector3 _cmdCamTargetPos;

        private Tile _lastStandingTile;
        private bool _isFallingdown; // Couldn't come up with a better idea

        public PlayerAgent(Transform transform, Transform camTransform) : base(transform)
        {
            _camTransform = camTransform;
            _cam = _camTransform.GetComponent<Camera>();
            _cmdCamTargetPos = _camTransform.position;
            _characterController = Transform.GetComponent<CharacterController>();

            // To make the player stay on the map with bends / unbends
            // Make it child of the tile it's currently standing on
            MessageBus.OnEvent<BendEvent>().Subscribe(ev =>
            {
                Transform.SetParent(_lastStandingTile.Transform);
                _camFollowCoeff = 10f; // Cam should snap when bending
                _camTransform.SetParent(Transform);
            });
            MessageBus.OnEvent<UnbendEvent>().Subscribe(ev =>
            {
                Transform.SetParent(_lastStandingTile.Transform);
                _camFollowCoeff = 1f;
                _camTransform.SetParent(Transform);
            });

            MessageBus.OnEvent<TweenCompletedEvent>().Subscribe(ev =>
            {
                Transform.SetParent(null);
                _camTransform.SetParent(null);
            });
        }

        protected override void OnMove(Vector3 dir)
        {
            base.OnMove(dir);

            // Only horizontal movement, for now
            _cmdMovDir = (_camTransform.rotation * dir).Horizontal().normalized;
        }

        protected override void OnCamZoom(float amount)
        {
            base.OnCamZoom(amount);
            var deltaZoom = amount * CamZoomSpeed * Time.deltaTime;

            _camFollowDistance -= deltaZoom;
            _camFollowDistance = Mathf.Clamp(_camFollowDistance, CamMinDistance, CamMaxDistance);
            _cam.orthographicSize = _camFollowDistance;
        }

        protected override void OnCamRotate(Vector2 dir)
        {
            base.OnCamRotate(dir);

            // Don't rotate too far
            dir = Vector2.ClampMagnitude(dir, 100);

            // Rotate the cam to be on the sphere around player
            var forward = 
                Quaternion.AngleAxis(dir.y * -CamRotateSpeed, _camTransform.right)
                * Quaternion.AngleAxis(dir.x * CamRotateSpeed, Vector3.up) 
                * _camTransform.forward
                * -1f; // Look down on the player

            var nextPos = Position + (forward * _camFollowDistance);

            // Make sure we don't overshoot "up" direction while rotating
            var d1 = Vector3.Cross((_camTransform.position - Position).normalized, Vector3.up);
            var d2 = Vector3.Cross((nextPos - Position).normalized, Vector3.up);
            if (Vector3.Dot(d1, d2) > 0)
            {
                _cmdCamTargetPos = nextPos;
            }
        }

        public override void Update()
        {
            base.Update();

            if (_cmdMovDir.sqrMagnitude > 0.001)
            {
                Transform.rotation = Quaternion.Slerp(Transform.rotation, 
                    Quaternion.LookRotation(_cmdMovDir, Vector3.up), Time.deltaTime * 15);
            }

            // Move and drag the camera with the same deltaMove
            var beforeMove = Position;
            _characterController.SimpleMove(_cmdMovDir * MaxSpeed * PfTime.DeltaTime);
            var deltaMove = Position - beforeMove;
            _cmdCamTargetPos += deltaMove;

            // PfTime isn't used here. Let the cam be independent of our custom timescale
            _camTransform.position = Vector3.Lerp(_camTransform.position, _cmdCamTargetPos,
                Time.deltaTime * _camFollowCoeff * 15); 

            _camTransform.LookAt(Position);

            // Store last standing tile
            RaycastHit hit;
            if (Physics.Raycast(Position, Vector3.down, out hit, 2f, 1 << Layer.Tile))
            {
                _lastStandingTile = hit.transform.GetComponent<Tile>();
            }

            // Fall check
            if (Position.y < -8 && !_isFallingdown)
            {
                _isFallingdown = true;
                MessageBus.Publish(new PlayerFallEvent());

                Util.WaitForSeconds(0.7f, () =>  // TODO: That magic number is haram
                {
                    _isFallingdown = false;

                    var dropAmount = (Position - _lastStandingTile.Position).magnitude;
                    Position = _lastStandingTile.Position + Vector3.up;

                    // When player is teleported back up, so should be camera
                    // ... with a little bit of extra leverage to make it look better
                    _cmdCamTargetPos += Vector3.up * (dropAmount + 10f);
                });
            }
        }


    }
}
