using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace BrutalCompanyPlus.Utils;

public static class BcpUtils {
    public static void InitializeNetcode() {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods) {
                var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                if (attributes.IsEmpty()) continue;

                Plugin.Logger.LogWarning($"Initializing RPCs for {type.Name}...");
                method.Invoke(null, null);
            }
        }
    }
    
    public static bool IsObjectTypeOf<T>(this SpawnableMapObject MapObject, out T Component) {
        Component = MapObject.prefabToSpawn.GetComponentInChildren<T>();
        return Component != null;
    }

    public static bool IsEmpty<T>(this IEnumerable<T> Collection) {
        return !Collection.Any();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // force inline
    public static void ForEach<T, TResult>(this IEnumerable<T> Collection, Func<T, TResult> Action) {
        foreach (var element in Collection) Action.Invoke(element);
    }
}