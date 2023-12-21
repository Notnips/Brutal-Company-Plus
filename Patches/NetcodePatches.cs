// ReSharper disable InconsistentNaming,RedundantAssignment

using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyPlus.Patches;

[HarmonyPatch]
internal static class NetcodePatches {
    [HarmonyPatch(typeof(GameNetworkManager), "Start")]
    [HarmonyPostfix]
    private static void InjectNetworkManager() {
        NetworkManager.Singleton.AddNetworkPrefab(Plugin.BCNetworkManagerPrefab);
    }

    [HarmonyPatch(typeof(StartOfRound), "Awake")]
    [HarmonyPostfix]
    private static void SpawnNetworkManager() {
        if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) return;
        Object.Instantiate(Plugin.BCNetworkManagerPrefab, Vector3.zero, Quaternion.identity)
            .GetComponent<NetworkObject>().Spawn(); // spawn network manager
    }
}