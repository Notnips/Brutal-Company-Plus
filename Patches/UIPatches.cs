// ReSharper disable InconsistentNaming,RedundantAssignment

using BrutalCompanyPlus.Utils;
using HarmonyLib;

namespace BrutalCompanyPlus.Patches;

[HarmonyPatch(typeof(MenuManager))]
internal static class UIPatches {
    [HarmonyPostfix, HarmonyPatch("Start")]
    private static void ShowDiagnosticsPatch(MenuManager __instance) {
        if (__instance.isInitScene || !Diagnostics.HasErrors) return;
        __instance.DisplayMenuNotification(
            $"[{PluginInfo.PLUGIN_NAME}]\n\nOne or more errors occurred during startup:\n\n{Diagnostics.CollectErrors()}",
            "[ Back ]");
    }
}