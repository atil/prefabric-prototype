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
            _controller.CamMove += OnCamMove;
            _controller.CamRotate += OnCamRotate;
        }

        protected virtual void OnMove(Vector3 dir) { }

        protected virtual void OnCamMove(Vector2 dir) { }

        protected virtual void OnCamRotate(Vector2 dir) { }

        public virtual void Update() { }
    }
}
