using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    public static bool IsEmpty<T>(this IEnumerable<T> Collection) {
        return !Collection.Any();
    }
}