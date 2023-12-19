using HarmonyLib;
using BrutalCompanyPlus.BCP;
using UnityEngine;
using System.Reflection.Emit;

namespace BrutalCompanyPlus.HarmPatches
{
    public static class LevelEventsPatches
    {

        public static bool test = false;
        public static bool test2 = false;
        public static bool test3 = false;

        [HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
        [HarmonyPrefix]
        private static bool ModifyLevel(ref SelectableLevel newLevel)
        {
            if (!RoundManager.Instance.IsHost)
            {
                return true;
            }

            //Variable and Object Cleanup
            Plugin.CleanupAndResetAI();

            //Check for existing enemies in current level before attempting to add missing enemies
            Plugin.UpdateAIInNewLevel(newLevel);

            //Configurable option to add all enemies to the level for spawn chance
            Plugin.AddMissingAIToNewLevel(newLevel);

            //Plugin.StoreOriginalEnemyList(newLevel); // Store the enemy list

            //Set previous modified enemy list back to original state
            //if (Variables.CurrentLevel != null)
            //{
            //    Plugin.RestoreOriginalEnemyList(Variables.CurrentLevel);
            //}

            Variables.CurrentLevel = newLevel;

            //Functions.LogEnemyList(newLevel);

            //Select our random event
            EventEnum eventEnum = Plugin.SelectRandomEvent();

            //EventEnum eventEnum = EventEnum.None;

            //Setup our Heat index for level and Enemy spawn list for later
            Plugin.InitializeLevelHeatValues(newLevel);

            //Adjust our Heat index to the level based on moon location and EventType
            Plugin.AdjustHeatValuesForAllLevels(eventEnum, newLevel);

            //Display our Heat Index to chat
            Variables.CurrentLevelHeatValue = Plugin.DisplayHeatMessages(newLevel);

            //Modifying enemy spawn rarity based on Heat Index
            //This is ugly and could prolly use a good redesign with better understanding of the rarity system
            Plugin.ApplyCustomLevelEnemyRarities(newLevel.Enemies, newLevel.name);

            //Ugly switch case to announce our event and adjust random things based on eventEnum (this can be cleaned up heavily)
            Plugin.HandleEventSpecificAdjustments(eventEnum, newLevel);

            //Modify the Animation Curve spawn count of Turrets/Landmines (Yuck)
            Plugin.UpdateMapObjects(newLevel, eventEnum);

            //Log info (needs updated)
            Plugin.LogEnemyInformation(newLevel);

            //Adjust Scrap Rates and Enemy Properties (Could be tweaked to be better honestly)
            Plugin.UpdateLevelProperties(newLevel);

            //Modify the Animation Curve spawn chance of Enemies (Yuck X2)
            Plugin.ModifyEnemySpawnChances(newLevel, eventEnum, Variables.CurrentLevelHeatValue);

            //Add Heat to level
            float NewHeatValue;
            Variables.levelHeatVal.TryGetValue(newLevel, out NewHeatValue);
            Variables.levelHeatVal[newLevel] = Mathf.Clamp(NewHeatValue + Plugin.MoonHeatIncreaseRate.Value, 0f, 100f);

            return true;
        }
    }
}
