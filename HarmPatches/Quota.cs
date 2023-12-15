using BrutalCompanyPlus.BCP;
using HarmonyLib;

namespace BrutalCompanyPlus.HarmPatches
{
    public static class QuotaPatches
    {
        [HarmonyPatch(typeof(TimeOfDay), "Awake")]
        [HarmonyPostfix]
        private static void ModifyTimeOfDay(TimeOfDay __instance)
        {
            Variables.mls.LogWarning("Modifying the Starting Quota Values based on Config");

            __instance.quotaVariables.deadlineDaysAmount = Plugin.DeadlineDaysAmount.Value;
            __instance.quotaVariables.startingCredits = Plugin.StartingCredits.Value;
            __instance.quotaVariables.startingQuota = Plugin.StartingQuota.Value;
            __instance.quotaVariables.baseIncrease = Plugin.BaseIncrease.Value;
        }
    }
}
