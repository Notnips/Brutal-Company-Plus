using BrutalCompanyPlus.BCP;
using HarmonyLib;
using System.Linq;
using UnityEngine;

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

        [HarmonyPatch(typeof(GameNetworkManager), "ResetSavedGameValues")]
        [HarmonyPostfix]
        public static void ResetMoonHeatOnFire(GameNetworkManager __instance)
        {
            foreach (SelectableLevel level in Variables.levelHeatVal.Keys.ToList<SelectableLevel>())
            {
                if (Variables.levelHeatVal.Count != 0)
                {
                    Variables.levelHeatVal[level] = 0f;
                }
            }
        }
        
        [HarmonyPatch(typeof(DeleteFileButton), "DeleteFile")]
        [HarmonyPostfix]
        public static void ResetMoonHeatOnDeleteFile(DeleteFileButton __instance)
        {
            foreach (SelectableLevel level in Variables.levelHeatVal.Keys.ToList<SelectableLevel>())
            {
                if (Variables.levelHeatVal.Count != 0)
                {
                    Variables.levelHeatVal[level] = 0f;
                }
            }
        }
    }
}
