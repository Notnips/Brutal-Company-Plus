// ReSharper disable InconsistentNaming,RedundantAssignment

using System.Linq;
using BrutalCompanyPlus.Objects;
using BrutalCompanyPlus.Utils;
using HarmonyLib;

namespace BrutalCompanyPlus.Patches;

[HarmonyPatch]
internal static class EnemyPatches {
    private const string Tag = $"[{nameof(EnemyPatches)}]";

    // Spawns all enemies that need to be spawned immediately.
    [HarmonyPrefix, HarmonyPatch(typeof(RoundManager), "BeginEnemySpawning")]
    private static void SpawnPendingEnemiesImmediately(ref RoundManager __instance) {
        if (EnemySpawnManager.PendingSpawns.IsEmpty()) return;
        foreach (var (enemyType, info) in EnemySpawnManager.PendingSpawns.TakeIf(si => si.SpawnInfo.Immediate)) {
            Plugin.Logger.LogInfo(
                $"{Tag} Spawning {info.Amount} {enemyType.enemyName} (outside: {info.Outside}) enemies... (immediate)");
            for (var i = 0; i < info.Amount; i++) {
                // If the enemy is an outside enemy, spawn it outside.
                if (info.Outside) EnemyUtils.SpawnOutsideEnemy(__instance, enemyType.enemyPrefab);
                // Otherwise, spawn it inside.
                else EnemyUtils.SpawnInsideEnemy(__instance, enemyType.enemyPrefab);
            }
        }
    }

    // Spawns all enemies that need to be spawned as soon as a player enters the facility.
    [HarmonyPrefix, HarmonyPatch(typeof(RoundManager), "SpawnInsideEnemiesFromVentsIfReady")]
    private static void SpawnPendingEnemiesDelayed(ref RoundManager __instance) {
        if (EnemySpawnManager.PendingSpawns.IsEmpty()) return;
        if (!StartOfRound.Instance.allPlayerScripts.Any(player => player.isInsideFactory)) return;
        foreach (var (enemyType, info) in EnemySpawnManager.PendingSpawns.TakeIf(si => !si.SpawnInfo.Immediate)) {
            Plugin.Logger.LogInfo(
                $"{Tag} Spawning {info.Amount} {enemyType.enemyName} (outside: {info.Outside}) enemies... (delayed)");
            for (var i = 0; i < info.Amount; i++) {
                // If the enemy is an outside enemy, spawn it outside.
                if (info.Outside) EnemyUtils.SpawnOutsideEnemy(__instance, enemyType.enemyPrefab);
                // Otherwise, spawn it inside.
                else EnemyUtils.SpawnInsideEnemy(__instance, enemyType.enemyPrefab);
            }
        }
    }
}