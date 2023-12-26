// ReSharper disable UnusedAutoPropertyAccessor.Global

using BepInEx.Configuration;
using BrutalCompanyPlus.Utils;

namespace BrutalCompanyPlus.Config;

public static class PluginConfig {
    [ConfigCategory("Custom Scrap Values")]
    [SharedDescription(
        $"Define min/max scrap pieces and min/max total scrap value for {{}} (for vanilla, use: {DefaultValues})")]
    public static class ScrapValues {
        public const string DefaultValues = "-1,-1,-1,-1";

        [Configuration("Experimentation", "6,25,400,1500")]
        public static ConfigEntry<string> ExperimentationLevel { get; set; }

        [Configuration("Assurance", "10,25,600,2500")]
        public static ConfigEntry<string> AssuranceLevel { get; set; }

        [Configuration("Vow", "12,35,600,2500")]
        public static ConfigEntry<string> VowLevel { get; set; }

        [Configuration("Offense", "15,35,800,3500")]
        public static ConfigEntry<string> OffenseLevel { get; set; }

        [Configuration("March", "15,35,800,3500")]
        public static ConfigEntry<string> MarchLevel { get; set; }

        [Configuration("Rend", "20,60,1500,5000")]
        public static ConfigEntry<string> RendLevel { get; set; }

        [Configuration("Dine", "20,60,1500,5000")]
        public static ConfigEntry<string> DineLevel { get; set; }

        [Configuration("Titan", "20,60,2000,6000")]
        public static ConfigEntry<string> TitanLevel { get; set; }

        [Configuration("any custom levels", "20,60,2000,6000")]
        public static ConfigEntry<string> CustomLevel { get; set; }
    }

    [ConfigCategory("Moon Heat")]
    public static class MoonHeat {
        [Configuration("Rate at which the moon heat increases by when landing back on the same planet", 20f)]
        public static ConfigEntry<float> IncreaseRate { get; set; }

        [Configuration("Rate at which the moon heat decreases by when not visiting the planet", 10f)]
        public static ConfigEntry<float> DecreaseRate { get; set; }

        [Configuration(
            "Defines how moon heat affects the weather (format: `start:end:type`, start is inclusive and end is exclusive)",
            "20:40:Rainy,40:60:Foggy,60:80:Flooded,80:100:Stormy,100:101:Eclipsed")]
        public static ConfigEntry<string> HeatCurve { get; set; }
    }

    [ConfigCategory("Map Hazard Adjustments")]
    public static class MapHazards {
        [Configuration("Amount of turrets that should be spawned (for vanilla, use: -1)", 8)]
        public static ConfigEntry<int> TurretSpawnRate { get; set; }

        [Configuration("Amount of landmines that should be spawned (for vanilla, use: -1)", 30)]
        public static ConfigEntry<int> LandmineSpawnRate { get; set; }
    }

    [ConfigCategory("Enemy Rarity Values")]
    [SharedDescription(
        "Define custom enemy rarity values for {} (no spawn: 0, max chance: 100, BCP default: -1)")]
    public static class EnemyRarityValues {
        // Adjust this accordingly every time a new enemy is added. Would be nice if this could be automated.
        private const string DefaultRarity =
            "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,Lasso:-1";

        [Configuration("Set this to `false` if you want vanilla spawning behavior.", true, true)]
        public static ConfigEntry<bool> Enabled { get; set; }

        [Configuration("Experimentation", DefaultRarity)]
        public static ConfigEntry<string> ExperimentationLevel { get; set; }

        [Configuration("Assurance", DefaultRarity)]
        public static ConfigEntry<string> AssuranceLevel { get; set; }

        [Configuration("Vow", DefaultRarity)] public static ConfigEntry<string> VowLevel { get; set; }

        [Configuration("Offense", DefaultRarity)]
        public static ConfigEntry<string> OffenseLevel { get; set; }

        [Configuration("March", DefaultRarity)]
        public static ConfigEntry<string> MarchLevel { get; set; }

        [Configuration("Rend", DefaultRarity)] public static ConfigEntry<string> RendLevel { get; set; }

        [Configuration("Dine", DefaultRarity)] public static ConfigEntry<string> DineLevel { get; set; }

        [Configuration("Titan", DefaultRarity)]
        public static ConfigEntry<string> TitanLevel { get; set; }

        [Configuration("any custom levels", DefaultRarity)]
        public static ConfigEntry<string> CustomLevel { get; set; }
    }

    [ConfigCategory("Credits-related Adjustments")]
    public static class CreditsAdjustments {
        [Configuration("Amount of money to give when a player leaves a moon alive (to disable, use: -1)",
            150)]
        public static ConfigEntry<int> FreeMoneyAmount { get; set; }
    }

    [ConfigCategory("Enemy-related Adjustments")]
    public static class EnemyAdjustments {
        [Configuration("Whether all enemies should be nominated for spawning on all moons (vanilla: false)",
            true)]
        public static ConfigEntry<bool> SpawnOnAllMoons { get; set; }
    }

    [ConfigCategory("Quota-related Adjustments")]
    public static class QuotaAdjustments {
        [Configuration("Days available before deadline (for vanilla, use: -1)", 4)]
        public static ConfigEntry<int> DeadlineDays { get; set; }

        [Configuration("Amount of credits everyone gets at the start of a new session (for vanilla, use: -1)",
            200)]
        public static ConfigEntry<int> StartingCredits { get; set; }

        [Configuration("Quota you begin with at the start of a new session (for vanilla, use: -1)", 400)]
        public static ConfigEntry<int> StartingQuota { get; set; }

        [Configuration("Rate at which the quota increases by when it's met (for vanilla, use: -1)", 275)]
        public static ConfigEntry<int> BaseIncrease { get; set; }
    }

    [ConfigCategory("Event Settings")]
    public static class EventSettings {
        [Configuration(
            "Chance between 0 and 100 for an event to occur (this is separate from event rarity and no events may still occur even if the chance is met!)",
            100, 0, 100)]
        public static ConfigEntry<int> GlobalChance { get; set; }

        [Configuration("Whether all events have equal chance to happen (disables event rarity altogether!)", false)]
        public static ConfigEntry<bool> EqualChance { get; set; }
    }

    public static void Bind(Plugin Plugin) {
        ConfigLoader.Bind(Plugin);
        Validate();
    }

    private static void Validate() {
        LevelNames.AllCustom.ForEach(ConfigUtils.GetEnemyRarityValues);
        LevelNames.AllCustom.ForEach(ConfigUtils.GetScrapValues);
        ConfigUtils.GetMoonHeatCurve();
    }
}