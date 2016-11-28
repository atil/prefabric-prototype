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
        private const float CamFollowDistance = 15f;
        private readonly Transform _camTransform;
        private Vector3 _cmdMovDir;

        private Vector3 _cmdCamTargetPos;
        private Quaternion _cmdCamTargetRot;

        public PlayerAgent(Transform transform, ControllerBase controller, Transform camTransform) : base(transform, controller)
        {
            _camTransform = camTransform;
            _cmdCamTargetPos = _camTransform.position;
            _characterController = Transform.GetComponent<CharacterController>();
        }
      
        protected override void OnMove(Vector3 dir)
        {
            base.OnMove(dir);
            _cmdMovDir = (_camTransform.rotation * dir).Horizontal().normalized;
        }

        protected override void OnCamMove(Vector2 dir)
        {
            base.OnCamMove(dir);
            dir.Normalize();

            _cmdCamTargetPos += _camTransform.right * dir.x * -CamMoveSpeed;
            _cmdCamTargetPos += _camTransform.up * dir.y * -CamMoveSpeed;
        }

        protected override void OnCamRotate(Vector2 dir)
        {
            base.OnCamRotate(dir);
            dir.Normalize();

            var forward = Quaternion.AngleAxis(dir.x * CamRotateSpeed, Vector3.up) * _camTransform.forward;
            _cmdCamTargetPos = Position + (-forward * CamFollowDistance);
            _cmdCamTargetRot = Quaternion.LookRotation(forward, Vector3.Cross(_camTransform.right, forward));
        }

        public override void Update()
        {
            base.Update();

            if (_cmdMovDir.sqrMagnitude > 0.001)
            {
                Transform.rotation = Quaternion.Slerp(Transform.rotation, Quaternion.LookRotation(_cmdMovDir, Vector3.up), Time.deltaTime * 15);
            }
            var grounded = _characterController.SimpleMove(_cmdMovDir * MaxSpeed * Time.deltaTime);

            _camTransform.position = Vector3.Lerp(_camTransform.position, _cmdCamTargetPos, Time.deltaTime * 15);
            _camTransform.rotation = Quaternion.Slerp(_camTransform.rotation, _cmdCamTargetRot, Time.deltaTime * 15);
        }


    }
}
