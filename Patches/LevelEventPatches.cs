// ReSharper disable InconsistentNaming,RedundantAssignment

using BrutalCompanyPlus.BCP;
using HarmonyLib;
using UnityEngine;

namespace BrutalCompanyPlus.Patches; 

[HarmonyPatch]
public static class LevelEventPatches {
    [HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
    [HarmonyPrefix]
    private static bool ModifyLevel(ref RoundManager __instance, ref SelectableLevel NewLevel) {
        if (!__instance.IsHost) return true;

        // //Set previous modified enemy list back to original state
        // //if (Variables.CurrentLevel != null)
        // //{
        // //    Functions.SetEnemyListOriginalState(Variables.CurrentLevel);
        // //}
        //
        // //Get the original enemy list and store it before modifying
        // //Functions.CloneEnemyList(newLevel);
        //
        // Variables.CurrentLevel = NewLevel;
        //
        // //Variable and Object Cleanup
        // Plugin.CleanupAndResetAI();
        //
        // //Check for existing enemies in current level before attempting to add missing enemies
        // Plugin.UpdateAIInNewLevel(NewLevel);
        //
        // //Configurable option to add all enemies to the level for spawn chance
        // Plugin.AddMissingAIToNewLevel(NewLevel);
        //
        // //Functions.LogEnemyList(newLevel);
        //
        // //Select our random event
        // var eventEnum = Plugin.SelectRandomEvent();
        //
        // //EventEnum eventEnum = EventEnum.None;
        //
        // //Setup our Heat index for level and Enemy spawn list for later
        // Plugin.InitializeLevelHeatValues(NewLevel);
        //
        // //Adjust our Heat index to the level based on moon location and EventType
        // Plugin.AdjustHeatValuesForAllLevels(eventEnum, NewLevel);
        //
        // //Display our Heat Index to chat
        // Variables.CurrentLevelHeatValue = Plugin.DisplayHeatMessages(NewLevel);
        //
        // //Modifying enemy spawn rarity based on Heat Index
        // //This is ugly and could prolly use a good redesign with better understanding of the rarity system
        // Plugin.ApplyCustomLevelEnemyRarities(NewLevel.Enemies, NewLevel.name);
        //
        // //Ugly switch case to announce our event and adjust random things based on eventEnum (this can be cleaned up heavily)
        // Plugin.HandleEventSpecificAdjustments(eventEnum, NewLevel);
        //
        // //Modify the Animation Curve spawn count of Turrets/Landmines (Yuck)
        // Plugin.UpdateMapObjects(NewLevel, eventEnum);
        //
        // //Log info (needs updated)
        // Plugin.LogEnemyInformation(NewLevel);
        //
        // //Adjust Scrap Rates and Enemy Properties (Could be tweaked to be better honestly)
        // Plugin.UpdateLevelProperties(NewLevel);
        //
        // //Modify the Animation Curve spawn chance of Enemies (Yuck X2)
        // Plugin.ModifyEnemySpawnChances(NewLevel, eventEnum, Variables.CurrentLevelHeatValue);
        //
        // //Add Heat to level
        // float newHeatValue;
        // Variables.LevelHeatVal.TryGetValue(NewLevel, out newHeatValue);
        // Variables.LevelHeatVal[NewLevel] = Mathf.Clamp(newHeatValue + Plugin.MoonHeatIncreaseRate.Value, 0f, 100f);

        return true;
    }
}