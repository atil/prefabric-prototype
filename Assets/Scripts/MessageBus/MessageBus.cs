using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prefabric
{
    /// <summary>
    /// Base class of all events used by messaging bus system
    /// </summary>
    public abstract class PfEvent { }

    /// <summary>
    /// Type of events which are meaningful within a level
    /// These type of events get unsubscribed with a level load call
    /// </summary>
    public abstract class PfSceneEvent : PfEvent { }

    /// <summary>
    /// A wrapper around UniRx.MessageBroker, which is a global event-subscribe system
    /// </summary>
    public static class MessageBus
    {
        /// <summary>
        /// Raises a custom-type event
        /// </summary>
        /// <typeparam name="T">System.Type of event</typeparam>
        /// <param name="evnt">Event instance</param>
        public static void Publish<T>(T evnt) where T : PfEvent
        {
            UniRx.MessageBroker.Default.Publish(evnt);
        }

        /// <summary>
        /// Subscribe to a PfEvent
        /// </summary>
        /// <typeparam name="T">Event type to listen to</typeparam>
        /// <returns>An IObservable which can be subscribed to</returns>
        public static UniRx.IObservable<T> OnEvent<T>() where T : PfEvent
        {
            return UniRx.MessageBroker.Default.Receive<T>();
        }

        /// <summary>
        /// Unsubscribe from all events which are inherited from PfSceneEvent
        /// This is needed because subscriptions from older scenes become problematic
        /// since those can point to older scene GameObjects (which are now destroyed)
        /// </summary>
        public static void ClearSceneEvents()
        {
            foreach (var type in Util.GetChildrenTypesOf<PfSceneEvent>())
            {
                UniRx.MessageBroker.Default.Remove(type);
            }
        }
    }
}
