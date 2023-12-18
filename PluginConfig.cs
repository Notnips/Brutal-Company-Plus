using System.Collections.Generic;
using BepInEx.Configuration;

namespace BrutalCompanyPlus;

public static class PluginConfig {
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

    // Configuration for adding all enemies to spawn list
    public static ConfigEntry<bool> EnableAllEnemy { get; private set; }

    // Configuration for Starting Quota Values
    public static ConfigEntry<int> DeadlineDaysAmount { get; private set; }
    public static ConfigEntry<int> StartingCredits { get; private set; }
    public static ConfigEntry<int> StartingQuota { get; private set; }
    public static ConfigEntry<float> BaseQuotaIncrease { get; private set; }

    // Configuration for Scrap Settings
    public static ConfigEntry<int> MinScrap { get; private set; }
    public static ConfigEntry<int> MaxScrap { get; private set; }
    public static ConfigEntry<int> MinTotalScrapValue { get; private set; }
    public static ConfigEntry<int> MaxTotalScrapValue { get; private set; }

    // Configuration for Event Chance
    // public static readonly Dictionary<EventEnum, ConfigEntry<int>> EventWeightEntries = new();

    public static void Bind(Plugin Plugin) {
        // Configuration for Factory Enemies
        FactoryStartOfDaySpawnChance = Plugin.Config.Bind("EnemySpawnSettings.Factory", "StartOfDaySpawnChance", -1f,
            "Factory enemy spawn chance at the start of the day. Set to -1 to use Brutals default value. (vanilla is around 2-5 depending on moon)");
        FactoryMidDaySpawnChance = Plugin.Config.Bind("EnemySpawnSettings.Factory", "MidDaySpawnChance", -1f,
            "Factory enemy spawn chance at midday. Set to -1 to use Brutals default value. (vanilla is around 5-10 depending on moon)");
        FactoryEndOfDaySpawnChance = Plugin.Config.Bind("EnemySpawnSettings.Factory", "EndOfDaySpawnChance", -1f,
            "Factory enemy spawn chance at the end of the day. Set to -1 to use Brutals default value. (vanilla is around 10-15 depending on moon)");

        // Configuration for Outside Enemies
        OutsideStartOfDaySpawnChance = Plugin.Config.Bind("EnemySpawnSettings.Outside", "StartOfDaySpawnChance", -1f,
            "Outside enemy spawn chance at the start of the day. Set to -1 to use default value. (vanilla is 0)");
        OutsideMidDaySpawnChance = Plugin.Config.Bind("EnemySpawnSettings.Outside", "MidDaySpawnChance", -1f,
            "Outside enemy spawn chance at midday. Set to -1 to use default value. (vanilla is 0.5)");
        OutsideEndOfDaySpawnChance = Plugin.Config.Bind("EnemySpawnSettings.Outside", "EndOfDaySpawnChance", -1f,
            "Outside enemy spawn chance at the end of the day. Set to -1 to use default value. (vanilla is 5)");

        MoonHeatDecreaseRate = Plugin.Config.Bind("MoonHeatSettings", "MoonHeatDecreaseRate", 10f,
            "Amount by which moon heat decreases when not visiting the planet");
        MoonHeatIncreaseRate = Plugin.Config.Bind("MoonHeatSettings", "MoonHeatIncreaseRate", 20f,
            "Amount by which moon heat increases when landing back on the same planet");

        EnableTurretModifications = Plugin.Config.Bind("MapObjectModificationSettings", "EnableTurretModifications",
            true, "Enable modifications to turret spawn rates on every moon, False would default to game logic");
        TurretSpawnRate = Plugin.Config.Bind("MapObjectModificationSettings", "TurretSpawnRate", 8f,
            "Default spawn amount for turrets on every moon");
        EnableLandmineModifications = Plugin.Config.Bind("MapObjectModificationSettings", "EnableLandmineModifications",
            true, "Enable modifications to landmine spawn rates on every moon, False would default to game logic");
        LandmineSpawnRate = Plugin.Config.Bind("MapObjectModificationSettings", "LandmineSpawnRate", 30f,
            "Default spawn amount for landmines on every moon");

        ExperimentationLevelRarities = Plugin.Config.Bind("CustomLevelRarities", "Experimentation",
            "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1",
            "Define custom enemy rarities for Experimentation (0 = no spawn, 100 = max chance, -1 = default Brutals rarity)");
        AssuranceLevelRarities = Plugin.Config.Bind("CustomLevelRarities", "Assurance",
            "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1",
            "Define custom enemy rarities for Assurance (0 = no spawn, 100 = max chance, -1 = default Brutals rarity)");
        VowLevelRarities = Plugin.Config.Bind("CustomLevelRarities", "Vow",
            "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1",
            "Define custom enemy rarities for Vow (0 = no spawn, 100 = max chance, -1 = default Brutals rarity)");
        MarchLevelRarities = Plugin.Config.Bind("CustomLevelRarities", "March",
            "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1",
            "Define custom enemy rarities for March (0 = no spawn, 100 = max chance, -1 = default Brutals rarity)");
        RendLevelRarities = Plugin.Config.Bind("CustomLevelRarities", "Rend",
            "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1",
            "Define custom enemy rarities for Rend (0 = no spawn, 100 = max chance, -1 = default Brutals rarity)");
        DineLevelRarities = Plugin.Config.Bind("CustomLevelRarities", "Dine",
            "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1",
            "Define custom enemy rarities for Dine (0 = no spawn, 100 = max chance, -1 = default Brutals rarity)");
        OffenseLevelRarities = Plugin.Config.Bind("CustomLevelRarities", "Offense",
            "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1",
            "Define custom enemy rarities for Offense (0 = no spawn, 100 = max chance, -1 = default Brutals rarity)");
        TitanLevelRarities = Plugin.Config.Bind("CustomLevelRarities", "Titan",
            "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1",
            "Define custom enemy rarities for Titan (0 = no spawn, 100 = max chance, -1 = default Brutals rarity)");

        EnableFreeMoney = Plugin.Config.Bind("EventOptions", "EnableFreeMoney", true,
            "This will give free money everytime survive and escape the planet");
        FreeMoneyValue = Plugin.Config.Bind("EventOptions", "FreeMoneyValue", 150,
            "This will control the amount of money you get when EnableFreeMoney is true");

        EnableAllEnemy = Plugin.Config.Bind("EnemySettings", "EnableAllEnemy", true,
            "This will add every enemy type to each moon as a spawn chance");

        DeadlineDaysAmount =
            Plugin.Config.Bind("QuotaSettings", "DeadlineDaysAmount", 4, "Days available before deadline");
        StartingCredits = Plugin.Config.Bind("QuotaSettings", "StartingCredits", 200,
            "Credits at the start of a new session");
        StartingQuota =
            Plugin.Config.Bind("QuotaSettings", "StartingQuota", 400, "Starting quota amount in a new session");
        BaseQuotaIncrease = Plugin.Config.Bind("QuotaSettings", "BaseIncrease", 275f,
            "Quota increase after meeting the previous quota");

        MinScrap = Plugin.Config.Bind("ScrapSettings", "MinScrap", 15, "Minimum scraps that can spawn on each moon");
        MaxScrap = Plugin.Config.Bind("ScrapSettings", "MaxScrap", 75, "Maximum scraps that can spawn on each moon");
        MinTotalScrapValue = Plugin.Config.Bind("ScrapSettings", "MinTotalScrapValue", 1500,
            "Minimum total scrap value on the moon");
        MaxTotalScrapValue = Plugin.Config.Bind("ScrapSettings", "MaxTotalScrapValue", 5000,
            "Maximum total scrap value on the moon");
    }
}