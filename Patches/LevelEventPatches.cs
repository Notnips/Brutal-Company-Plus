// ReSharper disable InconsistentNaming,RedundantAssignment

using BrutalCompanyPlus.Objects;
using HarmonyLib;

namespace BrutalCompanyPlus.Patches;

[HarmonyPatch(typeof(RoundManager))]
internal static class LevelEventPatches {
    [HarmonyPrefix, HarmonyPatch("LoadNewLevel")]
    private static bool ModifyLevel(ref RoundManager __instance, ref SelectableLevel newLevel) {
        // Make sure we're the host
        if (!__instance.IsHost) return true;
        // Adjust the level's heat values
        MoonHeatManager.AdjustHeatValues(newLevel);
        // Apply configured enemy rarity values
        LevelManager.ApplyEnemyRarityValues(newLevel);
        // Apply configured enemy spawn rates
        LevelManager.ApplyEnemySpawnRates(newLevel);
        // Apply configured level properties
        LevelManager.ApplyLevelProperties(newLevel);
        // Select a random event for this level and session
        EventManager.StartNewEvent(newLevel);
        // Finally, let the game load the level
        return true;
    }
}