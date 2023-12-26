// ReSharper disable InconsistentNaming,RedundantAssignment

using GameNetcodeStuff;
using HarmonyLib;

namespace BrutalCompanyPlus.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal static class PlayerPatches {
    [HarmonyPostfix, HarmonyPatch("Start")]
    private static void ShowDiagnosticsPatch(PlayerControllerB __instance) {
        // Originally, this is set to 129, while the enemy SFX is set to 128.
        // This causes the enemy SFX to be played over the player SFX because lower numbers have higher priority.
        __instance.movementAudio.priority = 127;
    }
}