using UnityEngine;

namespace Prefabric
{
    public class PlayerAgent : AgentBase
    {
        private const float MaxSpeed = 300f;
        private const float Gravity = 10f;
        private readonly CharacterController _characterController;

        private const float CamMoveSpeed = 1f;
        private const float CamRotateSpeed = 1f;
        private const float CamZoomSpeed = 400f;
        private const float CamMinDistance = 5f;
        private const float CamMaxDistance = 20f;

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

        public PlayerAgent(Transform transform, ControllerBase controller, Transform camTransform) : base(transform, controller)
        {
            _camTransform = camTransform;
            _cam = _camTransform.GetComponent<Camera>();
            _cmdCamTargetPos = _camTransform.position;
            _characterController = Transform.GetComponent<CharacterController>();
        }
      
        protected override void OnMove(Vector3 dir)
        {
            base.OnMove(dir);
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

            dir = Vector2.ClampMagnitude(dir, 100);

            // Rotate the cam to be on the sphere around player
            var forward = 
                Quaternion.AngleAxis(dir.y * -CamRotateSpeed, _camTransform.right)
                * Quaternion.AngleAxis(dir.x * CamRotateSpeed, Vector3.up) 
                * _camTransform.forward;
            _cmdCamTargetPos = Position + (-forward * _camFollowDistance);

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
            _characterController.SimpleMove(_cmdMovDir * MaxSpeed * Time.deltaTime);
            var deltaMove = Position - beforeMove;
            _cmdCamTargetPos += deltaMove;

            _camTransform.position = Vector3.Lerp(_camTransform.position, _cmdCamTargetPos, Time.deltaTime * 15);
            _camTransform.LookAt(Position);
        }


    }
}
