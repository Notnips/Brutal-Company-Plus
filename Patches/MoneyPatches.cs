// ReSharper disable InconsistentNaming,RedundantAssignment

using BrutalCompanyPlus.Utils;
using HarmonyLib;
using static BrutalCompanyPlus.Config.PluginConfig;

namespace BrutalCompanyPlus.Patches;

[HarmonyPatch]
internal static class MoneyPatches {
    [HarmonyPostfix, HarmonyPatch(typeof(TimeOfDay), "Awake")]
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

    [HarmonyPrefix, HarmonyPatch(typeof(RoundManager), "DetectElevatorIsRunning")]
    private static void CompensatePlayersPatch(ref RoundManager __instance) {
        if (!__instance.IsServer) return;

        // All players are dead, don't compensate
        if (StartOfRound.Instance.allPlayersDead) {
            ChatUtils.Send("<size=10><color=red>That was brutal! No compensation today :c</color></size>", true);
            return;
        }

        // All players are alive, compensate accordingly
        var amount = CreditsAdjustments.FreeMoneyAmount.Value;
        Singleton.Terminal.AddCredits(amount);
        ChatUtils.Send($"<size=10><color=green>You survived another day! Here's your compensation :)\n" +
                       $"<color=orange>+{amount} credits</color></size>", true);
    }
}