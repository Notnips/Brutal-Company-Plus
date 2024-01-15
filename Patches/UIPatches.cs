// ReSharper disable InconsistentNaming,RedundantAssignment

using BrutalCompanyPlus.Utils;
using HarmonyLib;
using UnityEngine.UI;

namespace BrutalCompanyPlus.Patches;

[HarmonyPatch(typeof(MenuManager))]
internal static class UIPatches {
    [HarmonyPostfix, HarmonyPatch("Start")]
    private static void ShowDiagnosticsPatch(MenuManager __instance) {
        if (__instance.isInitScene || !Diagnostics.HasErrors) return;

        // Show the error to the user
        __instance.DisplayMenuNotification(
            $"[{PluginInfo.PLUGIN_NAME}]\n" +
            $"One or more errors occurred during startup:\n" +
            $"{Diagnostics.CollectErrors()}\n\n",
            buttonText: "[ Quit ]"
        );
        // Quit the game once the user has acknowledged the error
        __instance.menuNotification.GetComponentInChildren<Button>().onClick.AddListener(__instance.ClickQuitButton);
    }
}