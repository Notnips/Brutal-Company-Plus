// ReSharper disable InconsistentNaming,RedundantAssignment,UseObjectOrCollectionInitializer

using BrutalCompanyPlus.Utils;
using GameNetcodeStuff;
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

    // Blobs are not designed to take damage from anything other than the player,
    // so we need to patch a bit of code so that it doesn't lag the game to a halt.
    [HarmonyPrefix, HarmonyPatch(typeof(BlobAI), "HitEnemy")]
    private static bool BlobDamageLagPatch(ref PlayerControllerB playerWhoHit) {
        return playerWhoHit != null;
    }
}