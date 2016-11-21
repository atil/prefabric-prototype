using UnityEngine;

namespace Prefabric
{
    public class PlayerAgent : AgentBase
    {
        public PlayerAgent(Transform transform, ControllerBase controller) : base(transform, controller)
        {

        }

        protected override void OnMove(Vector3 dir)
        {
            base.OnMove(dir);

            // transform.translate() etc. here
        }
    }
}
