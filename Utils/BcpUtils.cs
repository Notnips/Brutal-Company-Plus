using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DunGen;
using UnityEngine;

namespace BrutalCompanyPlus.Utils;

public static class BcpUtils {
    internal static void InitializeNetcode() {
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

    public static void AddCredits(this Terminal Terminal, int Amount) {
        Terminal.groupCredits += Amount;
        Terminal.SyncGroupCreditsServerRpc(Terminal.groupCredits, Terminal.numberOfItemsInDropship);
    }

    public static Vector3 GetNearbyLocation(Vector3 Location) {
        const float offsetDistance = 10.0f;
        var offset = UnityEngine.Random.insideUnitSphere * offsetDistance;
        return Location + offset;
    }

    public static GameObject FindObjectPrefab<T>(this SelectableLevel Level) where T : MonoBehaviour {
        foreach (var mapObject in Level.spawnableMapObjects) {
            if (!mapObject.IsObjectTypeOf<T>(out _)) continue;
            return mapObject.prefabToSpawn;
        }

        throw new Exception($"Unable to find prefab of type {typeof(T).Name} in level {Level.name}.");
    }

    public static bool IsObjectTypeOf<T>(this SpawnableMapObject MapObject, out T Component) {
        Component = MapObject.prefabToSpawn.GetComponentInChildren<T>();
        return Component != null;
    }

    public static bool IntInRange(this IntRange Range, int Value) =>
        Value >= Range.Min && Value <= Range.Max;

    public static bool IsEmpty<T>(this IEnumerable<T> Collection) => !Collection.Any();

    public static List<T> TakeIf<T>(this List<T> List, Func<T, bool> Predicate) {
        var items = List.Where(Predicate).ToList();
        List.RemoveAll(E => Predicate(E));
        return items;
    }

    public static bool Random<T>(this List<T> List, out T Value) {
        if (List.IsEmpty()) {
            Value = default;
            return false;
        }

        Value = List[UnityEngine.Random.Range(0, List.Count)];
        return true;
    }

    public static bool Random<T>(this T[] Array, out T Value) {
        if (Array.IsEmpty()) {
            Value = default;
            return false;
        }

        Value = Array[UnityEngine.Random.Range(0, Array.Length)];
        return true;
    }

    public static bool Random<T>(this IEnumerable<T> Enumerable, out T Value) =>
        Enumerable.ToList().Random(out Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // force inline
    public static void ForEach<T, TResult>(this IEnumerable<T> Collection, Func<T, TResult> Action) {
        foreach (var element in Collection) Action.Invoke(element);
    }
}