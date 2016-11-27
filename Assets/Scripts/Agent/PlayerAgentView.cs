using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prefabric
{
    public class EndZoneTriggeredEvent : PfEvent { }

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
