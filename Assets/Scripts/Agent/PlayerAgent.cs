using UnityEngine;

namespace Prefabric
{
    public class PlayerAgent : AgentBase
    {
        private const float MaxSpeed = 300f;
        private const float Gravity = 10f;
        private readonly CharacterController _characterController;
        private Vector3 _cmdMovDir;

        public PlayerAgent(Transform transform, ControllerBase controller) : base(transform, controller)
        {
            _characterController = Transform.GetComponent<CharacterController>();
        }

        protected override void OnMove(Vector3 dir)
        {
            base.OnMove(dir);
            _cmdMovDir = dir;
        }

        public override void Update()
        {
            base.Update();

            if (_cmdMovDir.sqrMagnitude > 0.001)
            {
                Transform.rotation = Quaternion.Slerp(Transform.rotation, Quaternion.LookRotation(_cmdMovDir, Vector3.up), Time.deltaTime * 15);
            }

            var grounded = _characterController.SimpleMove(_cmdMovDir * MaxSpeed * Time.deltaTime);
            
        }


    }
}
