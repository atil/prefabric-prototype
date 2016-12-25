using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prefabric
{
    public class EndZoneTriggeredEvent : PfSceneEvent { }

    /// <summary>
    /// We need player collider's Mono messages
    /// Attaching a MonoBehaviour to it seemed to be the most convenient way
    /// </summary>
    public class PlayerAgentView : MonoBehaviour
    {
        void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.layer == Layer.EndZone)
            {
                MessageBus.Publish(new EndZoneTriggeredEvent());
            }
        }
    }
}
