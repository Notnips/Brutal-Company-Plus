// ReSharper disable InconsistentNaming,RedundantAssignment

using BrutalCompanyPlus.Objects;
using HarmonyLib;

namespace BrutalCompanyPlus.Patches;

[HarmonyPatch]
internal static class LevelPatches {
    [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), "Start")]
    private static void ModifyLevelsPatch(ref StartOfRound __instance) {
        // Make sure we're the host
        if (!__instance.IsHost) return;
        // Add all enemies to all levels (if enabled)
        LevelManager.AddAllEnemiesToAllLevels();
        foreach (var level in __instance.levels) {
            // Initialize moon heat values
            MoonHeatManager.InitializeFor(level);
            // Apply configured enemy rarity values
            LevelManager.ApplyEnemyRarityValues(level);
            // Apply configured enemy spawn rates
            LevelManager.ApplyEnemySpawnRates(level);
            // Apply configured level properties
            LevelManager.ApplyLevelProperties(level);
        }
    }

    [HarmonyPrefix, HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
    private static void SelectLevelEventPatch(ref RoundManager __instance, ref SelectableLevel newLevel) {
        // Make sure we're the host
        if (!__instance.IsHost) return;
        // Adjust the level's heat values
        MoonHeatManager.AdjustHeatValues(newLevel);
        // Select a random event for this level and session
        EventManager.StartNewEvent(newLevel);
    }
}