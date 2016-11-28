using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    public class CameraView
    {
        private Transform _targetTransform;
        private float _distance;

        public Transform Transform { get; private set; }

        public CameraView(Transform tr)
        {
            Transform = tr;
            CoroutineStarter.StartCoroutine(MoveCoroutine());
        }

        public void Follow(AgentBase agent, float distance)
        {
            _targetTransform = agent.Transform;
            _distance = distance;
        }

        IEnumerator MoveCoroutine()
        {
            yield return null; // Wait for the first frame to finish
            while (true)
            {
                var targetPos = _targetTransform.position + (Transform.position - _targetTransform.position).normalized * _distance;
                Transform.position = Vector3.Lerp(Transform.position, targetPos, Time.deltaTime * 15);
                Transform.LookAt(_targetTransform);
                yield return null;
            }
        }
    }
}
