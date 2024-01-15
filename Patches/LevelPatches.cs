// ReSharper disable InconsistentNaming,RedundantAssignment

using BrutalCompanyPlus.Objects;
using BrutalCompanyPlus.Utils;
using HarmonyLib;

namespace BrutalCompanyPlus.Patches;

[HarmonyPatch]
internal static class LevelPatches {
    [HarmonyPostfix, HarmonyPriority(Priority.Last), HarmonyPatch(typeof(StartOfRound), "Start")]
    private static void ModifyLevelsPatch(ref StartOfRound __instance) {
        // Make sure we're the host
        if (!__instance.IsHost) return;
        // Add all enemies to all levels (if enabled)
        LevelManager.AddAllEnemiesToAllLevels(__instance.levels);
        foreach (var level in __instance.levels) {
            // Don't modify the Company Building
            if (level.name == LevelNames.CompanyBuilding) continue;
            // Initialize moon heat values
            MoonHeatManager.InitializeFor(level);
            // Apply configured enemy rarity values
            LevelManager.ApplyEnemyRarityValues(level);
            // Apply new enemy spawn chances
            LevelManager.ApplyEnemySpawnChances(level);
            // Apply configured enemy spawn rates
            LevelManager.ApplyEnemySpawnRates(level);
            // Apply configured level properties
            LevelManager.ApplyLevelProperties(level);
        }
    }

    // Ensure we're ran last, so other mods don't clear the chat.
    [HarmonyPrefix, HarmonyPriority(Priority.Last), HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
    private static void SelectLevelEventPatch(ref RoundManager __instance, ref SelectableLevel newLevel) {
        // Make sure we're the host
        if (!__instance.IsHost) return;

        // Don't do anything if we're at the Company Building
        if (newLevel.name == LevelNames.CompanyBuilding) {
            Plugin.Logger.LogWarning("Landed at the Company Building, forcing no event...");
            ChatUtils.Send("<color=green>Welcome to the Company Building!</color>", Clear: true);
            return;
        }

        // Adjust the level's heat values
        MoonHeatManager.AdjustHeatValues(newLevel);
        // Select a random event for this round
        EventManager.StartEventServer(newLevel);
    }

    [HarmonyPostfix, HarmonyPatch(typeof(RoundManager), "DespawnPropsAtEndOfRound")]
    private static void HandleLevelEventEndPatch(ref RoundManager __instance) {
        if (!__instance.IsHost) return;
        EventManager.EndEventServer(__instance.currentLevel);
    }
}