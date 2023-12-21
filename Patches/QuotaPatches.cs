// ReSharper disable InconsistentNaming,RedundantAssignment

using BrutalCompanyPlus.Utils;
using HarmonyLib;
using static BrutalCompanyPlus.Config.PluginConfig;

namespace BrutalCompanyPlus.Patches;

[HarmonyPatch(typeof(TimeOfDay))]
internal static class QuotaPatches {
    [HarmonyPostfix, HarmonyPatch("Awake")]
    private static void AdjustQuotaValues(ref TimeOfDay __instance) {
        Plugin.Logger.LogWarning("Adjusting starting quota values...");

        // deadline days
        if (QuotaAdjustments.DeadlineDays.GetIfSet(out var v))
            __instance.quotaVariables.deadlineDaysAmount = v;

        // starting credits
        if (QuotaAdjustments.StartingCredits.GetIfSet(out v))
            __instance.quotaVariables.startingCredits = v;

        // starting quota
        if (QuotaAdjustments.StartingQuota.GetIfSet(out v))
            __instance.quotaVariables.startingQuota = v;

        // base increase
        if (QuotaAdjustments.BaseIncrease.GetIfSet(out v))
            __instance.quotaVariables.baseIncrease = v;
    }
}