using UnityEngine;

namespace Prefabric
{
    public class PlayerAgent : AgentBase
    {
        private const float MaxSpeed = 10f;

        public PlayerAgent(Transform transform, ControllerBase controller) : base(transform, controller)
        {

        }

        protected override void OnMove(Vector3 dir)
        {
            base.OnMove(dir);

            Transform.Translate(dir * MaxSpeed * Time.deltaTime, Space.World);
            Transform.rotation = Quaternion.Slerp(Transform.rotation, Quaternion.LookRotation(dir, Vector3.up), Time.deltaTime * 15);
        }
    }
}
