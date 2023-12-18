// ReSharper disable InconsistentNaming,RedundantAssignment

using HarmonyLib;

namespace BrutalCompanyPlus.Patches;

[HarmonyPatch(typeof(TimeOfDay))]
public static class QuotaPatches {
    [HarmonyPostfix, HarmonyPatch("Awake")]
    private static void ModifyTimeOfDay(ref TimeOfDay __instance) {
        Plugin.Logger.LogWarning("Adjusting starting quota values...");
        __instance.quotaVariables.deadlineDaysAmount = PluginConfig.DeadlineDaysAmount.Value;
        __instance.quotaVariables.startingCredits = PluginConfig.StartingCredits.Value;
        __instance.quotaVariables.startingQuota = PluginConfig.StartingQuota.Value;
        __instance.quotaVariables.baseIncrease = PluginConfig.BaseQuotaIncrease.Value;
    }
}