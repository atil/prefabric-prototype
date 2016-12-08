﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prefabric
{
    public abstract class PfEvent { }

    public abstract class PfSceneEvent : PfEvent { }

    public static class MessageBus
    {
        public static void Publish<T>(T evnt) where T : PfEvent
        {
            UniRx.MessageBroker.Default.Publish(evnt);
        }

        public static UniRx.IObservable<T> OnEvent<T>() where T : PfEvent
        {
            return UniRx.MessageBroker.Default.Receive<T>();
        }

        public static void ClearSceneEvents()
        {
            foreach (var type in Util.GetChildrenTypesOf<PfSceneEvent>())
            {
                UniRx.MessageBroker.Default.Remove(type);
            }
        }
    }
}
