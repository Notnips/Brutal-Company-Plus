// ReSharper disable InconsistentNaming,RedundantAssignment

using BrutalCompanyPlus.Objects;
using BrutalCompanyPlus.Utils;
using HarmonyLib;

namespace BrutalCompanyPlus.Patches;

[HarmonyPatch]
internal static class EnemyPatches {
    private const string Tag = $"[{nameof(EnemyPatches)}]";

    // Spawns all enemies that need to be spawned immediately.
    [HarmonyPrefix, HarmonyPatch(typeof(RoundManager), "BeginEnemySpawning")]
    private static void SpawnPendingEnemies(ref RoundManager __instance) {
        if (!__instance.IsHost) return;
        foreach (var (enemyType, info) in EnemySpawnManager.PendingSpawns.TakeIf(si => si.SpawnInfo.Immediate)) {
            Plugin.Logger.LogInfo($"{Tag} Spawning {info.Amount} {enemyType.enemyName} enemies...");
            for (var i = 0; i < info.Amount; i++) {
                // If the enemy is an outside enemy, spawn it outside.
                if (info.Outside) EnemyUtils.SpawnOutsideEnemy(__instance, enemyType.enemyPrefab);
                // Otherwise, spawn it inside.
                else EnemyUtils.SpawnInsideEnemy(__instance, enemyType.enemyPrefab);
            }
        }
    }
}