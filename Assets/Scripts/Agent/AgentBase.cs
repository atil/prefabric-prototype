using UnityEngine;

namespace Prefabric
{
    public abstract class AgentBase
    {
        public Transform Transform { get; private set; }
        public Vector3 Position
        {
            get { return Transform.position; }
            set { Transform.position = value; }
        }

        protected readonly ControllerBase _controller;

        public AgentBase(Transform transform, ControllerBase controller)
        {
            Transform = transform;
            _controller = controller;

            _controller.Move += OnMove;
        }

        protected virtual void OnMove(Vector3 dir) { }
    }
}
