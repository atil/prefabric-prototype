﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabric
{
    /// <summary>
    /// A wrapper around Unity's LayerMask's static functions
    /// It's better to have those strings in one place only
    /// </summary>
    public static class Layer
    {
        public static LayerMask Tile { get; private set; }
        public static LayerMask EndZone { get; private set; }

        static Layer()
        {
            Tile = LayerMask.NameToLayer("Tile");
            EndZone = LayerMask.NameToLayer("EndZone");
        }

    }

    public static class Util
    {
        public static Vector3 Horizontal(this Vector3 v)
        {
            return Vector3.ProjectOnPlane(v, Vector3.up);
        }

        public static bool InBetween(this float val, float a, float b)
        {
            return (val < b && val > a) || (val < a && val > b);
        }

        public static T ToEnum<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static FieldInfo[] GetFieldsMarkedWith<T>(object obj) where T : Attribute
        {
            return obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttributes(typeof(T), true).Any()).ToArray();
        }

        public static Type[] GetChildrenTypesOf<T>()
        {
            return (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in domainAssembly.GetTypes()
                            where typeof(T).IsAssignableFrom(assemblyType) && assemblyType != typeof(T)
                            select assemblyType).ToArray();
        }

        public static void SetRgb(this Material mat, Color arg)
        {
            mat.color = new Color(arg.r, arg.g, arg.b, mat.color.a);
        }

        public static void SetAlpha(this Material m, float a)
        {
            var c = m.color;
            c.a = a;
            m.color = c;
        }

        public static void SetAlpha(this Image m, float a)
        {
            var c = m.color;
            c.a = a;
            m.color = c;
        }

        public static T Random<T>(this IList<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static void WaitForSeconds(float seconds, Action function)
        {
            Observable.Timer(TimeSpan.FromSeconds(seconds)).Subscribe(x => function());
        }

        public static bool Approx(float a, float b)
        {
            return Mathf.Abs(a - b) < 0.0001f;
        }

        public static string GetDataPath()
        {
#if UNITY_ANDROID
            return Application.persistentDataPath;
#else
            return Application.dataPath;
#endif

        }
    }
}
