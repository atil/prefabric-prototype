using UnityEngine;
using UniRx;

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

        public AgentBase(Transform transform)
        {
            Transform = transform;
            MessageBus.OnEvent<MoveCommand>().Subscribe(ev => OnMove(ev.Direction));
            MessageBus.OnEvent<CameraRotateCommand>().Subscribe(ev => OnCamRotate(ev.Amount));
            MessageBus.OnEvent<CameraZoomCommand>().Subscribe(ev => OnCamZoom(ev.Amount));
        }

        protected virtual void OnMove(Vector3 dir) { }

        protected virtual void OnCamRotate(Vector2 dir) { }

        protected virtual void OnCamZoom(float amount) { }

        public virtual void Update() { }
    }
}
