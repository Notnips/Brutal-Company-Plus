// ReSharper disable InconsistentNaming,RedundantAssignment,UseObjectOrCollectionInitializer

using BrutalCompanyPlus.Utils;
using HarmonyLib;
using UnityEngine;

namespace BrutalCompanyPlus.Patches;

[HarmonyPatch]
internal class SharedEnemyAIPatches {
    private static Vector3 ShipLocation => StartOfRound.Instance.playerSpawnPositions[0].position;

    // Modify starting position to start searching from the ship position
    [HarmonyPrefix, HarmonyPriority(Priority.First), HarmonyPatch(typeof(EnemyAI), "StartSearch")]
    private static void SpringManSearchFromShipPatch(ref EnemyAI __instance, ref Vector3 startOfSearch) {
        if (__instance is not SpringManAI || !__instance.isOutside) return;
        startOfSearch = BcpUtils.GetNearbyLocation(ShipLocation);
    }

    // Modify favorite position to be furthest away from the ship
    [HarmonyPostfix, HarmonyPatch(typeof(FlowermanAI), "Start")]
    private static void FlowermanOutsideFavoritePositionPatch(ref FlowermanAI __instance) {
        if (!__instance.isOutside) return;
        __instance.mainEntrancePosition = ShipLocation;
    }
}