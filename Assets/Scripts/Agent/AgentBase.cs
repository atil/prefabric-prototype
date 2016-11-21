using UnityEngine;

namespace Prefabric
{
    public abstract class AgentBase
    {
        public Transform Transform { get; private set; }

        public AgentBase(Transform transform)
        {
            Transform = transform;
        }
    }
}
