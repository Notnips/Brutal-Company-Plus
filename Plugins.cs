using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using UnityEngine;
using BrutalCompanyPlus.HarmPatches;
using BrutalCompanyPlus.BCP;
using BepInEx.Configuration;
using static BrutalCompanyPlus.BCP.MyPluginInfo;
using System.IO;
using System.Reflection.Emit;

namespace BrutalCompanyPlus
{

    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<string> ExperimentationLevelScrap { get; private set; }
        public static ConfigEntry<string> AssuranceLevelScrap { get; private set; }
        public static ConfigEntry<string> VowLevelScrap { get; private set; }
        public static ConfigEntry<string> MarchLevelScrap { get; private set; }
        public static ConfigEntry<string> RendLevelScrap { get; private set; }
        public static ConfigEntry<string> DineLevelScrap { get; private set; }
        public static ConfigEntry<string> OffenseLevelScrap { get; private set; }
        public static ConfigEntry<string> TitanLevelScrap { get; private set; }
        public static ConfigEntry<string> CustomLevelScrap { get; private set; }

        public static ConfigEntry<bool> VanillaRaritySettings { get; private set; }

        // Configuration for Factory Enemies Spawn Chance
        public static ConfigEntry<float> FactoryStartOfDaySpawnChance { get; private set; }
        public static ConfigEntry<float> FactoryMidDaySpawnChance { get; private set; }
        public static ConfigEntry<float> FactoryEndOfDaySpawnChance { get; private set; }

        // Configuration for Outside Enemies Spawn Chance
        public static ConfigEntry<float> OutsideStartOfDaySpawnChance { get; private set; }
        public static ConfigEntry<float> OutsideMidDaySpawnChance { get; private set; }
        public static ConfigEntry<float> OutsideEndOfDaySpawnChance { get; private set; }

        // Configuration for Moon Heat Settings
        public static ConfigEntry<float> MoonHeatDecreaseRate { get; private set; }
        public static ConfigEntry<float> MoonHeatIncreaseRate { get; private set; }

        // Configuration for Object Spawn Rate
        public static ConfigEntry<bool> EnableTurretModifications { get; private set; }
        public static ConfigEntry<bool> EnableLandmineModifications { get; private set; }
        public static ConfigEntry<float> TurretSpawnRate { get; private set; }
        public static ConfigEntry<float> LandmineSpawnRate { get; private set; }

        // Configuration for Factory Enemy Rarity
        public static ConfigEntry<string> ExperimentationLevelRarities { get; private set; }
        public static ConfigEntry<string> AssuranceLevelRarities { get; private set; }
        public static ConfigEntry<string> VowLevelRarities { get; private set; }
        public static ConfigEntry<string> MarchLevelRarities { get; private set; }
        public static ConfigEntry<string> RendLevelRarities { get; private set; }
        public static ConfigEntry<string> DineLevelRarities { get; private set; }
        public static ConfigEntry<string> OffenseLevelRarities { get; private set; }
        public static ConfigEntry<string> TitanLevelRarities { get; private set; }

        // Configuration for Free Money
        public static ConfigEntry<bool> EnableFreeMoney { get; private set; }
        public static ConfigEntry<int> FreeMoneyValue { get; private set; }

        // Configuration for Adding All enemies to Enemy List
        public static ConfigEntry<bool> EnableAllEnemy { get; private set; }

        // Configuration for Starting Quota Values
        public static ConfigEntry<int> DeadlineDaysAmount { get; private set; }
        public static ConfigEntry<int> StartingCredits { get; private set; }
        public static ConfigEntry<int> StartingQuota { get; private set; }
        public static ConfigEntry<float> BaseIncrease { get; private set; }

        // Configuration for Scrap Settings
        public static ConfigEntry<int> MinScrap { get; private set; }
        public static ConfigEntry<int> MaxScrap { get; private set; }
        public static ConfigEntry<int> MinTotalScrapValue { get; private set; }
        public static ConfigEntry<int> MaxTotalScrapValue { get; private set; }

        // Configuration for Event Chance
        public static Dictionary<EventEnum, ConfigEntry<int>> eventWeightEntries = new Dictionary<EventEnum, ConfigEntry<int>>();

        private bool brutalPlusInitialized = false;

        void Awake()
        {
            InitializeVariables();
            SetupPatches();
            InitializeBCP_ConfigSettings();
            InitializeDefaultEnemyRarities();

            //Depreciated Config Elements
            UpdateConfigurations();
            RemoveConfigSection(configFilePath, "EventEnabledConfig");
            RemoveConfigSection(configFilePath, "EnemySpawnSettings.Factory");
            RemoveConfigSection(configFilePath, "EnemySpawnSettings.Outside");
            RemoveConfigLines(configFilePath, "Chaos", "BadPlanet");

        }

        void Start()
        {
            if (!brutalPlusInitialized)
            {
                CreateBrutalPlusGameObject();
                brutalPlusInitialized = true;
            }
        }

        private void OnDestroy()
        {
            if (!brutalPlusInitialized)
            {
                CreateBrutalPlusGameObject();
                brutalPlusInitialized = true;
            }
        }

        public static List<string> correctEnemyNames = new List<string>
        {
            "Centipede", "Bunker Spider", "Hoarding bug", "Flowerman", "Crawler", "Blob", "Girl", "Puffer", "Nutcracker", "Spring", "Jester", "Masked", "LassoMan"
        };

        public static void UpdateConfigurations()
        {
            List<ConfigEntry<string>> levelConfigs = new List<ConfigEntry<string>>
            {
                ExperimentationLevelRarities, AssuranceLevelRarities, VowLevelRarities, MarchLevelRarities,
                RendLevelRarities, DineLevelRarities, OffenseLevelRarities, TitanLevelRarities
            };

            foreach (var configEntry in levelConfigs)
            {
                var existingConfigValues = ParseConfigRarities(configEntry.Value);
                var updatedConfigValues = new List<string>();

                foreach (var correctName in correctEnemyNames)
                {
                    string rarityValue = existingConfigValues.TryGetValue(correctName, out var existingRarity) ? existingRarity : "-1";
                    updatedConfigValues.Add($"{correctName}:{rarityValue}");
                }

                string updatedConfigValue = string.Join(",", updatedConfigValues);
                if (configEntry.Value != updatedConfigValue)
                {
                    configEntry.Value = updatedConfigValue;
                    BcpLogger.Log($"Updated configuration for {configEntry.Definition.Section}, {configEntry.Definition.Key}: {updatedConfigValue}");
                }
            }
        }

        private static Dictionary<string, string> ParseConfigRarities(string configValue)
        {
            return configValue.Split(',')
                              .Select(part => part.Split(':'))
                              .Where(parts => parts.Length == 2)
                              .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());
        }

        string configFilePath = Path.Combine(BepInEx.Paths.ConfigPath, "BrutalCompanyPlus.cfg");

        public void RemoveConfigLines(string filePath, params string[] variableNames)
        {
            try
            {
                var lines = File.ReadAllLines(filePath).ToList();
                var newLines = new List<string>();

                foreach (var line in lines)
                {
                    bool lineContainsVariable = variableNames.Any(variableName => line.Trim().StartsWith(variableName + " ="));

                    if (!lineContainsVariable)
                    {
                        newLines.Add(line); // Keep line if it doesn't contain the variable
                    }
                }

                File.WriteAllLines(filePath, newLines);
            }
            catch (Exception ex)
            {
                BcpLogger.Log($"Error occurred: {ex.Message}");
            }
        }

        public void RemoveConfigSection(string filePath, string sectionName)
        {
            try
            {
                var lines = File.ReadAllLines(filePath).ToList();
                var newLines = new List<string>();
                bool inSection = false;

                foreach (var line in lines)
                {
                    if (line.Trim().Equals($"[{sectionName}]"))
                    {
                        inSection = true; // Start skipping lines
                        continue;
                    }

                    if (inSection && line.StartsWith("[")) // Detect start of a new section
                    {
                        inSection = false; // End of target section, stop skipping
                    }

                    if (!inSection)
                    {
                        newLines.Add(line); // Keep line if it's not in the section
                    }
                }

                File.WriteAllLines(filePath, newLines);
            }
            catch (Exception ex)
            {
                BcpLogger.Log($"Error occurred: {ex.Message}");
            }
        }

        public static void InitializeDefaultEnemyRarities()
        {
            // Initialize default rarities for ExperimentationLevel
            var experimentationLevelRarities = new Dictionary<string, int>
            {
                { "Centipede", 50 }, { "Bunker Spider", 75 }, { "Hoarding bug", 80 }, { "Flowerman", 30 }, { "Crawler", 15 }, { "Blob", 25 }, { "Girl", 2 },
                { "Puffer", 10 }, { "Nutcracker", 15 }, { "Spring", 5 }, { "Jester", 1 }, { "Masked", 1 }, { "LassoMan", 1 }
            };
            Variables.DefaultEnemyRarities["ExperimentationLevel"] = experimentationLevelRarities;

            // Initialize default rarities for AssuranceLevel
            var assuranceLevelRarities = new Dictionary<string, int>
            {
                { "Centipede", 50 }, { "Bunker Spider", 40 }, { "Hoarding bug", 50 }, { "Flowerman", 30 }, { "Crawler", 15 }, { "Blob", 25 }, { "Girl", 2 },
                { "Puffer", 40 }, { "Nutcracker", 15 }, { "Spring", 25 }, { "Jester", 3 }, { "Masked", 3 }, { "LassoMan", 1 }
            };
            Variables.DefaultEnemyRarities["AssuranceLevel"] = assuranceLevelRarities;

            // Initialize default rarities for "VowLevel": 
            var vowLevelRarities = new Dictionary<string, int>
            {
                { "Centipede", 50 }, { "Bunker Spider", 40 }, { "Hoarding bug", 50 }, { "Flowerman", 30 }, { "Crawler", 20 }, { "Blob", 25 }, { "Girl", 5 },
                { "Puffer", 40 }, { "Nutcracker", 20 }, { "Spring", 40 }, { "Jester", 15 }, { "Masked", 10 }, { "LassoMan", 0 }
            };
            Variables.DefaultEnemyRarities["VowLevel"] = vowLevelRarities;

            // Initialize default rarities for "OffenseLevel": 
            var offenseLevelRarities = new Dictionary<string, int>
            {
                { "Centipede", 50 }, { "Bunker Spider", 40 }, { "Hoarding bug", 50 }, { "Flowerman", 30 }, { "Crawler", 20 }, { "Blob", 25 }, { "Girl", 5 },
                { "Puffer", 40 }, { "Nutcracker", 20 }, { "Spring", 40 }, { "Jester", 15 }, { "Masked", 10 }, { "LassoMan", 0 }
            };
            Variables.DefaultEnemyRarities["OffenseLevel"] = offenseLevelRarities;

            // Initialize default rarities for "MarchLevel": 
            var marchLevelRarities = new Dictionary<string, int>
            {
                { "Centipede", 50 }, { "Bunker Spider", 40 }, { "Hoarding bug", 50 }, { "Flowerman", 30 }, { "Crawler", 20 }, { "Blob", 25 }, { "Girl", 10 },
                { "Puffer", 40 }, { "Nutcracker", 20 }, { "Spring", 40 }, { "Jester", 15 }, { "Masked", 10 }, { "LassoMan", 0 }
            };
            Variables.DefaultEnemyRarities["MarchLevel"] = marchLevelRarities;

            // Initialize default rarities for "RendLevel": 
            var rendLevelRarities = new Dictionary<string, int>
            {
                { "Centipede", 35 }, { "Bunker Spider", 25 }, { "Hoarding bug", 30 }, { "Flowerman", 56 }, { "Crawler", 50 }, { "Blob", 40 }, { "Girl", 25 },
                { "Puffer", 40 }, { "Nutcracker", 30 }, { "Spring", 58 }, { "Jester", 40 }, { "Masked", 10 }, { "LassoMan", 2 }
            };
            Variables.DefaultEnemyRarities["RendLevel"] = rendLevelRarities;

            // Initialize default rarities for "DineLevel": 
            var dineLevelRarities = new Dictionary<string, int>
            {
                { "Centipede", 35 }, { "Bunker Spider", 25 }, { "Hoarding bug", 30 }, { "Flowerman", 56 }, { "Crawler", 50 }, { "Blob", 40 }, { "Girl", 25 },
                { "Puffer", 40 }, { "Nutcracker", 30 }, { "Spring", 58 }, { "Jester", 40 }, { "Masked", 10 }, { "LassoMan", 2 }
            };
            Variables.DefaultEnemyRarities["DineLevel"] = dineLevelRarities;

            // Initialize default rarities for "TitanLevel": 
            var titanLevelRarities = new Dictionary<string, int>
            {
                { "Centipede", 35 }, { "Bunker Spider", 25 }, { "Hoarding bug", 30 }, { "Flowerman", 56 }, { "Crawler", 60 }, { "Blob", 40 }, { "Girl", 25 },
                { "Puffer", 40 }, { "Nutcracker", 30 }, { "Spring", 58 }, { "Jester", 40 }, { "Masked", 10 }, { "LassoMan", 2 }
            };
            Variables.DefaultEnemyRarities["TitanLevel"] = titanLevelRarities;
        }

        public void InitializeBCP_ConfigSettings()
        {
            ExperimentationLevelScrap = Config.Bind("CustomScrapSettings", "ExperimentationLevelScrap", "6,25,400,1500", "Define min/max scrap pieces and min/max total scrap value for Experimentation Level ( -1 = Vanilla Value )");
            AssuranceLevelScrap = Config.Bind("CustomScrapSettings", "AssuranceLevelScrap", "10,25,600,2500", "Define min/max scrap pieces and min/max total scrap value for Assurance Level ( -1 = Vanilla Value )");
            VowLevelScrap = Config.Bind("CustomScrapSettings", "VowLevelScrap", "12,35,600,2500", "Define min/max scrap pieces and min/max total scrap value for Vow Level ( -1 = Vanilla Value )");
            OffenseLevelScrap = Config.Bind("CustomScrapSettings", "OffenseLevelScrap", "15,35,800,3500", "Define min/max scrap pieces and min/max total scrap value for Offense Level ( -1 = Vanilla Value )");
            MarchLevelScrap = Config.Bind("CustomScrapSettings", "MarchLevelScrap", "15,35,800,3500", "Define min/max scrap pieces and min/max total scrap value for March Level ( -1 = Vanilla Value )");
            RendLevelScrap = Config.Bind("CustomScrapSettings", "RendLevelScrap", "20,60,1500,5000", "Define min/max scrap pieces and min/max total scrap value for Rend Level ( -1 = Vanilla Value )");
            DineLevelScrap = Config.Bind("CustomScrapSettings", "DineScrap", "20,60,1500,5000", "Define min/max scrap pieces and min/max total scrap value for Dine Level ( -1 = Vanilla Value )");
            TitanLevelScrap = Config.Bind("CustomScrapSettings", "TitanLevelScrap", "20,60,2000,6000", "Define min/max scrap pieces and min/max total scrap value for Titan Level ( -1 = Vanilla Value )");
            CustomLevelScrap = Config.Bind("CustomScrapSettings", "CustomLevelScrap", "-1,-1,-1,-1", "Define min/max scrap pieces and min/max total scrap value for Any Custom Levels ( -1 = Vanilla Value )");

            MoonHeatDecreaseRate = Config.Bind("MoonHeatSettings", "MoonHeatDecreaseRate", 10f, "Amount by which moon heat decreases when not visiting the planet");
            MoonHeatIncreaseRate = Config.Bind("MoonHeatSettings", "MoonHeatIncreaseRate", 20f, "Amount by which moon heat increases when landing back on the same planet");

            EnableTurretModifications = Config.Bind("MapObjectModificationSettings", "EnableTurretModifications", true, "Enable modifications to turret spawn rates on every moon, False would default to game logic");
            TurretSpawnRate = Config.Bind("MapObjectModificationSettings", "TurretSpawnRate", 8f, "Default spawn amount for turrets on every moon");
            EnableLandmineModifications = Config.Bind("MapObjectModificationSettings", "EnableLandmineModifications", true, "Enable modifications to landmine spawn rates on every moon, False would default to game logic");
            LandmineSpawnRate = Config.Bind("MapObjectModificationSettings", "LandmineSpawnRate", 30f, "Default spawn amount for landmines on every moon");

            VanillaRaritySettings = Config.Bind("CustomLevelRarities", "VanillaRaritySettings", false, "If TRUE this will DISABLE overwriting Rarity Values for Factory Enemies");
            ExperimentationLevelRarities = Config.Bind("CustomLevelRarities", "Experimentation", "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1", "Define custom enemy rarities for Experimentation (0 = no spawn, 100 = max chance, -1 = default Brutals rarity, The spawn rate will also be impacted depnding on the Chance rate)");
            AssuranceLevelRarities = Config.Bind("CustomLevelRarities", "Assurance", "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1", "Define custom enemy rarities for Assurance (0 = no spawn, 100 = max chance, -1 = default Brutals rarity, The spawn rate will also be impacted depnding on the Chance rate)");
            VowLevelRarities = Config.Bind("CustomLevelRarities", "Vow", "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1", "Define custom enemy rarities for Vow (0 = no spawn, 100 = max chance, -1 = default Brutals rarity, The spawn rate will also be impacted depnding on the Chance rate)");
            MarchLevelRarities = Config.Bind("CustomLevelRarities", "March", "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1", "Define custom enemy rarities for March (0 = no spawn, 100 = max chance, -1 = default Brutals rarity, The spawn rate will also be impacted depnding on the Chance rate)");
            RendLevelRarities = Config.Bind("CustomLevelRarities", "Rend", "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1", "Define custom enemy rarities for Rend (0 = no spawn, 100 = max chance, -1 = default Brutals rarity, The spawn rate will also be impacted depnding on the Chance rate)");
            DineLevelRarities = Config.Bind("CustomLevelRarities", "Dine", "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1", "Define custom enemy rarities for Dine (0 = no spawn, 100 = max chance, -1 = default Brutals rarity, The spawn rate will also be impacted depnding on the Chance rate)");
            OffenseLevelRarities = Config.Bind("CustomLevelRarities", "Offense", "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1", "Define custom enemy rarities for Offense (0 = no spawn, 100 = max chance, -1 = default Brutals rarity, The spawn rate will also be impacted depnding on the Chance rate)");
            TitanLevelRarities = Config.Bind("CustomLevelRarities", "Titan", "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1", "Define custom enemy rarities for Titan (0 = no spawn, 100 = max chance, -1 = default Brutals rarity, The spawn rate will also be impacted depnding on the Chance rate)");

            EnableFreeMoney = Config.Bind("EventOptions", "EnableFreeMoney", true, "This will give free money everytime survive and escape the planet");
            FreeMoneyValue = Config.Bind("EventOptions", "FreeMoneyValue", 150, "This will control the amount of money you get when EnableFreeMoney is true");

            EnableAllEnemy = Config.Bind("EnemySettings", "EnableAllEnemy", true, "This will add every vanilla enemy type to each moon as a spawn chance");

            DeadlineDaysAmount = Config.Bind("QuotaSettings", "DeadlineDaysAmount", 4, "Days available before deadline");
            StartingCredits = Config.Bind("QuotaSettings", "StartingCredits", 200, "Credits at the start of a new session");
            StartingQuota = Config.Bind("QuotaSettings", "StartingQuota", 400, "Starting quota amount in a new session");
            BaseIncrease = Config.Bind("QuotaSettings", "BaseIncrease", 275f, "Quota increase after meeting the previous quota");

            MinScrap = Config.Bind("ScrapSettings", "MinScrap", 10, "Minimum scraps that can spawn on each moon");
            MaxScrap = Config.Bind("ScrapSettings", "MaxScrap", 40, "Maximum scraps that can spawn on each moon");
            MinTotalScrapValue = Config.Bind("ScrapSettings", "MinTotalScrapValue", 600, "Minimum total scrap value on the moon");
            MaxTotalScrapValue = Config.Bind("ScrapSettings", "MaxTotalScrapValue", 5000, "Maximum total scrap value on the moon");

            eventWeightEntries[EventEnum.None] = Config.Bind("EventChanceConfig", "None", 100, "[Nothing Happened Today] Nothing special will happen (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.Turret] = Config.Bind("EventChanceConfig", "Turret", 100, "[Turret Terror] This will spawn turrets all over the place inside the factory (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.Delivery] = Config.Bind("EventChanceConfig", "Delivery", 100, "[ICE SCREAM] This will order between 3 - 9 random items from the shop (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.BlobEvolution] = Config.Bind("EventChanceConfig", "BlobEvolution", 100, "[They have EVOLVED] This will spawn Blobs inside and 1 outside and they can open doors and move much faster (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.Guards] = Config.Bind("EventChanceConfig", "Guards", 100, "[They gaurd this place] this will spawn nutcrackers outside (Set Chance between 0 -100)");
            eventWeightEntries[EventEnum.SurfaceExplosion] = Config.Bind("EventChanceConfig", "SurfaceExplosion", 100, "[The Surface is explosive] Mines wills spawn at the feet of players not in the ship or factory, they also have a delayed fuse (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.FaceHuggers] = Config.Bind("EventChanceConfig", "FaceHuggers", 100, "[Bring a shovel] This will spawn MANY Centipedes into the factory (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.TheRumbling] = Config.Bind("EventChanceConfig", "TheRumbling", 100, "[The Rumbling] This will spawn MANY Forest Giants when the ship has fully landed (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.TheyWant2Play] = Config.Bind("EventChanceConfig", "TheyWant2Play", 100, "[The just want to play] This will spawn several Ghost girls into the level (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.BeastInside] = Config.Bind("EventChanceConfig", "BeastInside", 100, "[The Beasts Inside] This will spawn Eyeless Dogs into the Factory, spawn rate changes depending on moon (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.ShadowRealm] = Config.Bind("EventChanceConfig", "ShadowRealm", 100, "[The shadows are roaming] This will spawn several bracken outside along with a chance of Fog (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.Unfair] = Config.Bind("EventChanceConfig", "Unfair", 100, "[UNFAIR COMPANY] This will spawn several outside enemies and inside enemies at a significant rate (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.OutsideBox] = Config.Bind("EventChanceConfig", "OutsideBox", 100, "[Outside the box] This will spawn Jesters Outside (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.InstaJester] = Config.Bind("EventChanceConfig", "InstaJester", 100, "[Pop goes the... HOLY FUC-] This will spawn several jesters that have a short crank timer between 0 - 10 seconds instead of 30 - 45 seconds (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.InsideOut] = Config.Bind("EventChanceConfig", "InsideOut", 100, "[Spring Escape] This will spawn Coil heads outside, they will instantly roam around the ship (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.Landmine] = Config.Bind("EventChanceConfig", "Landmine", 100, "[Minescape Terror] This will spawn MANY landmines inside the factory (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.Sacrifice] = Config.Bind("EventChanceConfig", "Sacrifice", 100, "[The Hunger Games?] This will rotate through players at a given rate, when the selected player steps inside the factory.. They get choosen for death. (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.ShipTurret] = Config.Bind("EventChanceConfig", "ShipTurret", 100, "[When did we get this installed?!?] This will spawn a turret inside the ship facing the controls (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.HoardTown] = Config.Bind("EventChanceConfig", "HoardTown", 100, "[Hoarder Town] This will spawn MANY Hoarder Bugs inside the factory and outside (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.TheyAreShy] = Config.Bind("EventChanceConfig", "TheyAreShy", 100, "[Don't look... away!] This spawn several Spring Heads and Brackens inside & outside the factory (Set Chance between 0 - 100)");
            eventWeightEntries[EventEnum.ResetHeat] = Config.Bind("EventChanceConfig", "ResetHeat", 100, "[All Moons Heat Reset] This will reset the moon heat for all moons (Set Chance between 0 - 100)");
        }

        private void InitializeVariables()
        {
            Variables.mls = BepInEx.Logging.Logger.CreateLogSource(PLUGIN_NAME);

            Variables.mls.LogWarning("Loaded Brutal Company Plus and applying patches.");

            Variables.mls = base.Logger;

            Variables.levelHeatVal = new Dictionary<SelectableLevel, float>();

            Variables.enemyRaritys = new Dictionary<SpawnableEnemyWithRarity, int>();

            Variables.levelEnemySpawns = new Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>>();

            Variables.enemyPropCurves = new Dictionary<SpawnableEnemyWithRarity, AnimationCurve>();

        }

        private void SetupPatches()
        {
            Variables._harmony.PatchAll(typeof(LevelEventsPatches));

            Variables._harmony.PatchAll(typeof(QuotaPatches));

            Variables._harmony.PatchAll(typeof(EnemyAIPatches));
        }

        private void CreateBrutalPlusGameObject()
        {
            try
            {
                Variables.mls.LogInfo("Attempting to Initialize BrutalPlus");
                GameObject brutalplus = new GameObject("BrutalPlus");

                // Make sure the GameObject is active
                brutalplus.SetActive(true);

                BrutalPlus brutalPlusComponent = brutalplus.AddComponent<BrutalPlus>();

                // Ensure the component is enabled
                brutalPlusComponent.enabled = true;

                // Prevent the GameObject from being destroyed on load
                DontDestroyOnLoad(brutalplus);

                Variables.loaded = true;
            }
            catch (Exception ex)
            {
                Variables.mls.LogInfo("Exception in CreateBrutalPlusGameObject: " + ex.Message);
                BcpLogger.Log("Exception in CreateBrutalPlusGameObject: " + ex.Message);
            }
        }

        public static void CleanupAndResetAI()
        {
            Functions.CleanUpAllVariables();
            BrutalPlus.CleanupAllSpawns();
        }

        public static void UpdateAIInNewLevel(SelectableLevel newLevel)
        {
            foreach (var enemy in newLevel.Enemies)
            {
                var enemyPrefab = enemy.enemyType.enemyPrefab;
                foreach (var aiType in Variables.aiPresence.Keys.ToList())
                {
                    if (enemyPrefab.GetComponent(aiType) != null)
                    {
                        Variables.aiPresence[aiType] = true;
                    }
                }
            }
        }

        public static void AddMissingAIToNewLevel(SelectableLevel newLevel)
        {
            //Config value to setup all enemies inside
            if (EnableAllEnemy.Value)
            {
                SelectableLevel[] levels = StartOfRound.Instance.levels;
                foreach (var level in levels)
                {
                    foreach (var spawnable in level.Enemies)
                    {
                        var enemyPrefab = spawnable.enemyType.enemyPrefab;
                        foreach (var aiType in Variables.aiPresence.Keys.ToList())
                        {
                            if (enemyPrefab.GetComponent(aiType) != null && !Variables.aiPresence[aiType])
                            {
                                Variables.aiPresence[aiType] = true;
                                newLevel.Enemies.Add(spawnable);
                                Variables.mls.LogWarning($"\n\nAdded Enemy: > {spawnable.enemyType.enemyPrefab.name} < to the Enemy list\n\n");
                                //BcpLogger.Log($"Added Enemy: > {spawnable.enemyType.enemyPrefab.name} < to the Enemy list");
                            }
                        }
                    }
                }
            }
        }

        public static EventEnum SelectRandomEvent()
        {
            var weightedEvents = eventWeightEntries
                .Where(kvp => kvp.Value.Value > 0) // Only consider events with weight > 0
                .Select(kvp => new { Event = kvp.Key, Weight = kvp.Value.Value })
                .ToList();

            if (weightedEvents.Count == 0)
            {
                return EventEnum.None;
            }

            int totalWeight = weightedEvents.Sum(e => e.Weight);
            int randomNumber = UnityEngine.Random.Range(0, totalWeight);
            int weightSum = 0;

            foreach (var weightedEvent in weightedEvents)
            {
                weightSum += weightedEvent.Weight;
                if (randomNumber < weightSum)
                {
                    Variables.lastEvent = weightedEvent.Event;
                    return weightedEvent.Event;
                }
            }

            return EventEnum.None; // Fallback, should not normally reach here
        }


        public static void ApplyCustomLevelEnemyRarities(List<SpawnableEnemyWithRarity> enemies, string levelName)
        {
            if (VanillaRaritySettings.Value)
            {
                return;
            }

            try
            {
                BcpLogger.Log($"Applying custom rarities for level: {levelName}");

                var levelConfig = GetConfigForLevel(levelName);
                BcpLogger.Log($"Config for level {levelName}: {levelConfig?.Value}");

                if (levelConfig == null || string.IsNullOrWhiteSpace(levelConfig.Value))
                {
                    BcpLogger.Log($"No custom config found or config is empty for level {levelName}. Using default rarities.");
                    return;
                }

                var customRarities = levelConfig.Value.Split(',')
                                                    .Select(r => r.Split(':'))
                                                    .Where(r => r.Length == 2)
                                                    .ToDictionary(r => r[0].Trim(), r => int.Parse(r[1]));

                if (!Variables.DefaultEnemyRarities.TryGetValue(levelName, out var levelDefaultRarities))
                {
                    BcpLogger.Log($"No default rarities found for level {levelName}");
                    return;
                }

                foreach (var enemy in enemies)
                {
                    if (customRarities.TryGetValue(enemy.enemyType.enemyName, out int customRarity))
                    {
                        if (customRarity != -1)
                        {
                            BcpLogger.Log($"Setting custom rarity for {enemy.enemyType.enemyName} to {customRarity}");
                            enemy.rarity = customRarity;
                        }
                        else if (levelDefaultRarities.TryGetValue(enemy.enemyType.enemyName, out int defaultRarity))
                        {
                            BcpLogger.Log($"Using default rarity for {enemy.enemyType.enemyName}: {defaultRarity}");
                            enemy.rarity = defaultRarity;
                        }
                    }
                    else if (levelDefaultRarities.TryGetValue(enemy.enemyType.enemyName, out int defaultRarity))
                    {
                        BcpLogger.Log($"No custom rarity for {enemy.enemyType.enemyName}. Using default rarity: {defaultRarity}");
                        enemy.rarity = defaultRarity;
                    }
                    else
                    {
                        BcpLogger.Log($"No rarity information found for {enemy.enemyType.enemyName}. Skipping.");
                    }
                }
            }
            catch (Exception e)
            {
                BcpLogger.Log($"Exception in ApplyCustomLevelEnemyRarities: {e.ToString()}");
            }
        }

        private static ConfigEntry<string> GetConfigForLevel(string levelName)
        {
            switch (levelName)
            {
                case "ExperimentationLevel": return ExperimentationLevelRarities;
                case "AssuranceLevel": return AssuranceLevelRarities;
                case "VowLevel": return VowLevelRarities;
                case "MarchLevel": return MarchLevelRarities;
                case "RendLevel": return RendLevelRarities;
                case "DineLevel": return DineLevelRarities;
                case "OffenseLevel": return OffenseLevelRarities;
                case "TitanLevel": return TitanLevelRarities;
                default: return null;
            }
        }

        public static void UpdateLevelEnemies(SelectableLevel newLevel, float MoonHeat)
        {
            List<SpawnableEnemyWithRarity> enemies;
            if (Variables.levelEnemySpawns.TryGetValue(newLevel, out enemies))
            {
                ApplyCustomLevelEnemyRarities(enemies, newLevel.name);
                newLevel.Enemies = enemies;
            }
        }

        public static void InitializeLevelHeatValues(SelectableLevel newLevel)
        {
            if (!Variables.levelHeatVal.ContainsKey(newLevel))
            {
                Variables.levelHeatVal.Add(newLevel, 0f);
            }

            if (!Variables.levelEnemySpawns.ContainsKey(newLevel))
            {
                List<SpawnableEnemyWithRarity> list = new List<SpawnableEnemyWithRarity>();
                foreach (SpawnableEnemyWithRarity item in newLevel.Enemies)
                {
                    list.Add(item);
                }
                Variables.levelEnemySpawns.Add(newLevel, list);
            }
        }

        public static void AdjustHeatValuesForAllLevels(EventEnum eventEnum, SelectableLevel newLevel)
        {
            foreach (SelectableLevel level in Variables.levelHeatVal.Keys.ToList<SelectableLevel>())
            {
                if (newLevel != level)
                {
                    float HeatValue;
                    Variables.levelHeatVal.TryGetValue(level, out HeatValue);
                    Variables.levelHeatVal[level] = Mathf.Clamp(HeatValue - MoonHeatDecreaseRate.Value, 0f, 100f);
                }

                if (eventEnum == EventEnum.ResetHeat)
                {
                    Variables.levelHeatVal[level] = 0f;
                }
            }
        }

        public static float DisplayHeatMessages(SelectableLevel newLevel)
        {
            float HeatValue;
            Variables.levelHeatVal.TryGetValue(newLevel, out HeatValue);
            HUDManager.Instance.AddTextToChatOnServer("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", -1);
            HUDManager.Instance.AddTextToChatOnServer("<color=orange>MOON IS AT " + HeatValue.ToString() + "% HEAT</color>", -1);
            if (HeatValue >= 20f && HeatValue < 40)
            {
                HUDManager.Instance.AddTextToChatOnServer("<size=11><color=blue>Moonheat is rising and caused it to rain. <color=white>\nVisit other moons to decrease your moon heat!</color></size>", -1);
                //newLevel.currentWeather = LevelWeatherType.Rainy;
            }

            if (HeatValue >= 40f && HeatValue < 60)
            {
                HUDManager.Instance.AddTextToChatOnServer("<size=11><color=purple>Moonheat is rising causing a layer of fog. <color=white>\nVisit other moons to decrease your moon heat!</color></size>", -1);
                //newLevel.currentWeather = LevelWeatherType.Foggy;
            }

            if (HeatValue >= 60f && HeatValue < 80)
            {
                HUDManager.Instance.AddTextToChatOnServer("<size=11><color=yellow>Moonheat is HOT, flooding the moon to cool it off. <color=white>\nVisit other moons to decrease your moon heat!</color></size>", -1);
                //newLevel.currentWeather = LevelWeatherType.Flooded;
            }

            if (HeatValue >= 80f && HeatValue < 100)
            {
                HUDManager.Instance.AddTextToChatOnServer("<size=11><color=orange>Extremely high moonheat detected causing dangerous weather. <color=white>\nVisit other moons to decrease your moon heat!</color></size>", -1);
                //newLevel.currentWeather = LevelWeatherType.Stormy;
            }

            if (HeatValue >= 100)
            {
                HUDManager.Instance.AddTextToChatOnServer("<size=11><color=red>The Moon is overheated and now hostile creatures roam it. <color=white>\nVisit other moons to decrease your moon heat!</color></size>", -1);
                //newLevel.currentWeather = LevelWeatherType.Eclipsed;
            }
            return HeatValue;
        }

        public static void HandleEventSpecificAdjustments(EventEnum eventEnum, SelectableLevel newLevel)
        {

            if (newLevel.sceneName == "CompanyBuilding")
            {
                HUDManager.Instance.AddTextToChatOnServer("<color=green>Welcome to the Company Buidling!</color>", -1);
                return;
            }

            switch (eventEnum)
            {
                case EventEnum.None:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=green>Nothing Happened Today!</color>", -1);
                    break;


                case EventEnum.Turret:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>Turret Terror</color>", -1);
                    break;


                case EventEnum.Landmine:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>Minescape Terror</color>", -1);
                    break;


                case EventEnum.InsideOut:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>Spring Escape!</color>", -1);
                    Variables.SpawnInsideOut = true;
                    break;


                //case EventEnum.SmiteMe:
                //    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>He's angry with you</color>", -1);
                //    newLevel.currentWeather = LevelWeatherType.Stormy;
                //    Variables.SmiteEnabled = true;
                //    break;


                case EventEnum.HoardTown:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>Hoarder Town</color>", -1);
                    Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(HoarderBugAI), 8, SpawnLocation.Vent, false, false));
                    Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(HoarderBugAI), 8, SpawnLocation.Outside, false, true));
                    break;


                case EventEnum.ShadowRealm:
                    string AdditionalInfo = "";
                    if (UnityEngine.Random.Range(0, 1) == 1)
                    {
                        newLevel.currentWeather = LevelWeatherType.Foggy;
                        AdditionalInfo = "in the mist!";
                    }
                    HUDManager.Instance.AddTextToChatOnServer($"<color=yellow>EVENT<color=white>:</color></color>\n<color=red>The shadows are roaming {AdditionalInfo}</color>", -1);
                    Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(FlowermanAI), 6, SpawnLocation.Outside, false, true));
                    break;


                case EventEnum.BeastInside:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>The Beasts Inside!</color>", -1);
                    Functions.TheBeastsInside();
                    break;


                case EventEnum.TheRumbling:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>The Rumbling!</color>", -1);
                    Functions.TheRumbling();
                    Variables.TheRumbling = true;
                    break;


                case EventEnum.Sacrifice:
                    HUDManager.Instance.AddTextToChatOnServer($"<color=yellow>EVENT<color=white>:</color></color>\n<color=red>The Hunger Games?</color>", -1);
                    Variables.Tribute = true;
                    break;


                case EventEnum.OutsideBox:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>Outside the box!</color>", -1);
                    Functions.FindEnemyPrefabByType(typeof(JesterAI), newLevel.Enemies, newLevel);
                    Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(JesterAI), 2, SpawnLocation.Outside, false, true));
                    break;

                case EventEnum.Guards:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>They gaurd this place!</color>", -1);
                    Functions.FindEnemyPrefabByType(typeof(NutcrackerEnemyAI), newLevel.Enemies, newLevel);
                    Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(NutcrackerEnemyAI), 4, SpawnLocation.Outside, false, true));
                    break;


                case EventEnum.Unfair:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>UNFAIR COMPANY</color>", -1);
                    break;


                case EventEnum.FaceHuggers:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>Bring a Shovel!!</color>", -1);
                    Variables.WaitUntilPlayerInside = true;
                    Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(CentipedeAI), 15, SpawnLocation.Vent, false, false));
                    break;


                case EventEnum.BlobEvolution:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>They are EVOLVING?!?</color>", -1);
                    Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(BlobAI), 3, SpawnLocation.Vent, false, false));
                    Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(BlobAI), 1, SpawnLocation.Outside, false, true));
                    Variables.BlobsHaveEvolved = true;
                    break;


                case EventEnum.Delivery:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=green>ICE SCREAM!!!</color>", -1);
                    int RandomAmount = UnityEngine.Random.Range(2, 9);
                    for (int i = 0; i < RandomAmount; i++)
                    {
                        int RandomItem = UnityEngine.Random.Range(0, 12);
                        UnityEngine.Object.FindObjectOfType<Terminal>().orderedItemsFromTerminal.Add(RandomItem);
                    }
                    break;


                case EventEnum.InstaJester:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>Pop goes the.. HOLY FUC- </color>", -1);
                    //Ensure or add Jester to Moon
                    Functions.FindEnemyPrefabByType(typeof(JesterAI), newLevel.Enemies, newLevel);
                    //Spawn Jester
                    Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(JesterAI), 2, SpawnLocation.Vent, false, false));

                    Variables.InstaJester = true;
                    break;


                case EventEnum.TheyAreShy:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>Don't look... away?</color>", -1);
                    Functions.FindEnemyPrefabByType(typeof(FlowermanAI), newLevel.Enemies, newLevel);
                    Functions.FindEnemyPrefabByType(typeof(SpringManAI), newLevel.Enemies, newLevel);
                    Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(SpringManAI), 3, SpawnLocation.Vent, false, false));
                    Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(FlowermanAI), 3, SpawnLocation.Vent, false, false));
                    Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(FlowermanAI), 3, SpawnLocation.Outside, false, true));
                    Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(SpringManAI), 2, SpawnLocation.Outside, false, true));

                    break;


                case EventEnum.TheyWant2Play:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>They just wants to play!!</color>", -1);

                    //Ensure or add DressGirlAI to Moon
                    Functions.FindEnemyPrefabByType(typeof(DressGirlAI), newLevel.Enemies, newLevel);

                    Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(DressGirlAI), 5, SpawnLocation.Vent, false, false));
                    break;


                case EventEnum.SurfaceExplosion:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>The surface is explosive</color>", -1);
                    Variables.slSpawnTimer = -10f;
                    Variables.surpriseLandmines += 120;
                    break;


                case EventEnum.ResetHeat:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=green>All Moons Heat Reset</color>", -1);
                    // Logic for Reset Heat event
                    break;


                case EventEnum.ShipTurret:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=red>When did we get this installed?!?</color>", -1);
                    Variables.shouldSpawnTurret = true;
                    break;


                default:
                    HUDManager.Instance.AddTextToChatOnServer("<color=yellow>EVENT<color=white>:</color></color>\n<color=green>Nothing happened today!</color>", -1);
                    break;
            }
        }

        public static void StoreOriginalEnemyList(SelectableLevel level)
        {
            if (!Variables.originalEnemyLists.ContainsKey(level))
            {
                Variables.originalEnemyLists[level] = new List<SpawnableEnemyWithRarity>(level.Enemies);
            }
        }

        public static void ModifyCurrentLevelEnemies(SelectableLevel currentLevel, params Type[] keepEnemyTypes)
        {
            // Set all types in defaultRemovableEnemyTypes to false
            foreach (var key in Variables.defaultRemovableEnemyTypes.Keys.ToList())
            {
                Variables.defaultRemovableEnemyTypes[key] = false;
            }

            // Set passed enemy types to true (to keep them)
            // Add them to the dictionary if they don't exist
            foreach (var type in keepEnemyTypes)
            {
                if (Variables.defaultRemovableEnemyTypes.ContainsKey(type))
                {
                    Variables.defaultRemovableEnemyTypes[type] = true;
                }
                else
                {
                    Variables.defaultRemovableEnemyTypes.Add(type, true);
                }
            }

            // Remove enemies from current level based on defaultRemovableEnemyTypes
            currentLevel.Enemies.RemoveAll(enemy =>
                Variables.defaultRemovableEnemyTypes.TryGetValue(enemy.enemyType.GetType(), out bool keep) && !keep
            );
        }

        public static void RestoreOriginalEnemyList(SelectableLevel level)
        {
            if (Variables.originalEnemyLists.ContainsKey(level))
            {
                level.Enemies = Variables.originalEnemyLists[level];
                Variables.originalEnemyLists.Remove(level); // Optionally remove the stored list if it's no longer needed
            }
        }

        //This adjust landmines and turrets for each level
        public static void UpdateMapObjects(SelectableLevel newLevel, EventEnum eventEnum)
        {
            foreach (SpawnableMapObject spawnableMapObject in newLevel.spawnableMapObjects)
            {
                // Check if the map object is a Turret
                if (spawnableMapObject.prefabToSpawn.GetComponentInChildren<Turret>() != null)
                {
                    Variables.turret = spawnableMapObject.prefabToSpawn;
                    if (eventEnum == EventEnum.Turret)
                    {
                        spawnableMapObject.numberToSpawn = new AnimationCurve(new Keyframe[]
                        {
                    new Keyframe(0f, 200f),
                    new Keyframe(1f, 25f)
                        });
                    }
                    else if (EnableTurretModifications.Value)
                    {
                        spawnableMapObject.numberToSpawn = new AnimationCurve(new Keyframe[]
                        {
                    new Keyframe(0f, 0f),
                    new Keyframe(1f, TurretSpawnRate.Value)
                        });
                    }
                }
                // Check if the map object is a Landmine
                else if (spawnableMapObject.prefabToSpawn.GetComponentInChildren<Landmine>() != null)
                {
                    Variables.landmine = spawnableMapObject.prefabToSpawn;
                    if (eventEnum == EventEnum.Landmine)
                    {
                        spawnableMapObject.numberToSpawn = new AnimationCurve(new Keyframe[]
                        {
                    new Keyframe(0f, 300f),
                    new Keyframe(1f, 170f)
                        });
                    }
                    else if (EnableLandmineModifications.Value)
                    {
                        spawnableMapObject.numberToSpawn = new AnimationCurve(new Keyframe[]
                        {
                    new Keyframe(0f, 0f),
                    new Keyframe(1f, LandmineSpawnRate.Value)
                        });
                    }
                }

                // Log the map object for debugging
                Variables.mls.LogInfo(spawnableMapObject.prefabToSpawn.ToString());
            }
        }

        //Log information
        public static void LogEnemyInformation(SelectableLevel newLevel)
        {
            Variables.mls.LogWarning("Map Objects");
            foreach (SpawnableMapObject spawnableMapObject in newLevel.spawnableMapObjects)
            {
                Variables.mls.LogInfo(spawnableMapObject.prefabToSpawn.ToString());
            }
            Variables.mls.LogWarning("Enemies");
            foreach (SpawnableEnemyWithRarity spawnableEnemy in newLevel.Enemies)
            {
                Variables.mls.LogInfo(spawnableEnemy.enemyType.enemyName + "--rarity = " + spawnableEnemy.rarity.ToString());
            }
            // And similarly for Daytime Enemies...
        }

        //This adjust the value and total of items on the map
        public static void UpdateLevelProperties(SelectableLevel newLevel)
        {
            // Add the new level to the modified levels list if not already present
            if (!Variables.levelsModified.Contains(newLevel))
            {
                Variables.levelsModified.Add(newLevel);

                var scrapSettings = GetConfigForScrapSettings(newLevel.name)?.Value.Split(',');
                if (scrapSettings != null && scrapSettings.Length == 4)
                {
                    // Use per-level configuration, default to vanilla settings if -1
                    newLevel.minScrap = scrapSettings[0] != "-1" ? int.Parse(scrapSettings[0]) : newLevel.minScrap;
                    newLevel.maxScrap = scrapSettings[1] != "-1" ? int.Parse(scrapSettings[1]) : newLevel.maxScrap;
                    newLevel.minTotalScrapValue = scrapSettings[2] != "-1" ? int.Parse(scrapSettings[2]) : newLevel.minTotalScrapValue;
                    newLevel.maxTotalScrapValue = scrapSettings[3] != "-1" ? int.Parse(scrapSettings[3]) : newLevel.maxTotalScrapValue;
                }

                newLevel.maxEnemyPowerCount += 150;
                newLevel.maxOutsideEnemyPowerCount += 10;
                newLevel.maxDaytimeEnemyPowerCount += 150;
            }
            // Other level property adjustments can be added here
        }

        private static ConfigEntry<string> GetConfigForScrapSettings(string levelName)
        {
            switch (levelName)
            {
                case "ExperimentationLevel": return ExperimentationLevelScrap;
                case "AssuranceLevel": return AssuranceLevelScrap;
                case "VowLevel": return VowLevelScrap;
                case "MarchLevel": return MarchLevelScrap;
                case "RendLevel": return RendLevelScrap;
                case "DineLevel": return DineLevelScrap;
                case "OffenseLevel": return OffenseLevelScrap;
                case "TitanLevel": return TitanLevelScrap;
                default: return CustomLevelScrap;
            }
        }

        public static void ModifyEnemySpawnChances(SelectableLevel newLevel, EventEnum eventEnum, float MoonHeat)
        {
            newLevel.enemySpawnChanceThroughoutDay = new AnimationCurve((Keyframe[])(object)new Keyframe[3]
            {
                new Keyframe(0f, 0.1f),
                new Keyframe(0.5f, 10f),
                new Keyframe(1f, 70f)
            });

            newLevel.outsideEnemySpawnChanceThroughDay = new AnimationCurve((Keyframe[])(object)new Keyframe[3]
            {
                new Keyframe(0f, -30f),
                new Keyframe(20f, -20f),
                new Keyframe(21f, 10f)
            });

            // Adjust spawn chances based on the event
            switch (eventEnum)
            {
                case EventEnum.Unfair:
                    newLevel.outsideEnemySpawnChanceThroughDay = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
                    {
                        new Keyframe(0f, 999f),
                        new Keyframe(21f, 999f)
                    });
                    newLevel.enemySpawnChanceThroughoutDay = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
                    {
                        new Keyframe(0f, 500f),
                        new Keyframe(0.5f, 500f)
                    });
                    break;

                default:
                    // Default adjustments or no adjustments
                    break;
            }

            // Additional adjustments to spawn chances can be added here
        }

    }
}
