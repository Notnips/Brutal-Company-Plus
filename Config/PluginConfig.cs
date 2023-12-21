﻿using BepInEx.Configuration;
using BrutalCompanyPlus.Utils;

namespace BrutalCompanyPlus.Config;

public static class PluginConfig {
    [ConfigCategory("Custom Scrap Values")]
    [SharedDescription(
        $"Define min/max scrap pieces and min/max total scrap value for {{}} (for vanilla, use: {DefaultValues})")]
    public static class ScrapValues {
        public const string DefaultValues = "-1,-1,-1,-1";

        [Configuration<string>("Experimentation", "6,25,400,1500")]
        public static ConfigEntry<string> ExperimentationLevel => null;

        [Configuration<string>("Assurance", "6,25,400,1500")]
        public static ConfigEntry<string> AssuranceLevel => null;

        [Configuration<string>("Vow", "6,25,400,1500")]
        public static ConfigEntry<string> VowLevel => null;

        [Configuration<string>("Offense", "6,25,400,1500")]
        public static ConfigEntry<string> OffenseLevel => null;

        [Configuration<string>("March", "6,25,400,1500")]
        public static ConfigEntry<string> MarchLevel => null;

        [Configuration<string>("Rend", "6,25,400,1500")]
        public static ConfigEntry<string> RendLevel => null;

        [Configuration<string>("Dine", "6,25,400,1500")]
        public static ConfigEntry<string> DineLevel => null;

        [Configuration<string>("Titan", "6,25,400,1500")]
        public static ConfigEntry<string> TitanLevel => null;

        [Configuration<string>("any custom levels", "6,25,400,1500")]
        public static ConfigEntry<string> CustomLevel => null;
    }

    [ConfigCategory("Moon Heat")]
    public static class MoonHeat {
        [Configuration<float>("Rate at which the moon heat increases by when landing back on the same planet", 20f)]
        public static ConfigEntry<float> IncreaseRate => null;

        [Configuration<float>("Rate at which the moon heat decreases by when not visiting the planet", 10f)]
        public static ConfigEntry<float> DecreaseRate => null;

        [Configuration<string>(
            "Defines how moon heat affects the weather (format: `start:end:type`, start is inclusive and end is exclusive)",
            "20:40:Rainy,40:60:Foggy,60:80:Flooded,80:100:Stormy,100:101:Eclipsed")]
        public static ConfigEntry<string> HeatCurve => null;
    }

    [ConfigCategory("Map Hazard Adjustments")]
    public static class MapHazards {
        [Configuration<int>("Amount of turrets that should be spawned (for vanilla, use: -1)", 8)]
        public static ConfigEntry<int> TurretSpawnRate => null;

        [Configuration<int>("Amount of landmines that should be spawned (for vanilla, use: -1)", 30)]
        public static ConfigEntry<int> LandmineSpawnRate => null;
    }

    [ConfigCategory("Enemy Rarity Values")]
    [SharedDescription(
        "Define custom enemy rarity values for {} (no spawn: 0, max chance: 100, BCP default: -1)")]
    public static class EnemyRarityValues {
        // Adjust this accordingly every time a new enemy is added. Would be nice if this could be automated.
        private const string DefaultRarity =
            "Centipede:-1,Bunker Spider:-1,Hoarding bug:-1,Flowerman:-1,Crawler:-1,Blob:-1,Girl:-1,Puffer:-1,Nutcracker:-1,Spring:-1,Jester:-1,Masked:-1,LassoMan:-1";

        [Configuration<bool>("Set this to `false` if you want vanilla spawning behavior.", true, true)]
        public static ConfigEntry<bool> Enabled => null;

        [Configuration<string>("Experimentation", DefaultRarity)]
        public static ConfigEntry<string> ExperimentationLevel => null;

        [Configuration<string>("Assurance", DefaultRarity)]
        public static ConfigEntry<string> AssuranceLevel => null;

        [Configuration<string>("Vow", DefaultRarity)]
        public static ConfigEntry<string> VowLevel => null;

        [Configuration<string>("Offense", DefaultRarity)]
        public static ConfigEntry<string> OffenseLevel => null;

        [Configuration<string>("March", DefaultRarity)]
        public static ConfigEntry<string> MarchLevel => null;

        [Configuration<string>("Rend", DefaultRarity)]
        public static ConfigEntry<string> RendLevel => null;

        [Configuration<string>("Dine", DefaultRarity)]
        public static ConfigEntry<string> DineLevel => null;

        [Configuration<string>("Titan", DefaultRarity)]
        public static ConfigEntry<string> TitanLevel => null;

        [Configuration<string>("any custom levels", DefaultRarity)]
        public static ConfigEntry<string> CustomLevel => null;
    }

    [ConfigCategory("Credits-related Adjustments")]
    public static class CreditsAdjustments {
        [Configuration<int>("Amount of money to give when a player leaves a moon alive (to disable, use: -1)",
            150)]
        public static ConfigEntry<int> FreeMoneyAmount => null;
    }

    [ConfigCategory("Enemy-related Adjustments")]
    public static class EnemyAdjustments {
        [Configuration<bool>("Whether all enemies should be nominated for spawning on all moons (vanilla: false)",
            true)]
        public static ConfigEntry<bool> SpawnOnAllMoons => null;
    }

    [ConfigCategory("Quota-related Adjustments")]
    public static class QuotaAdjustments {
        [Configuration<int>("Days available before deadline (for vanilla, use: -1)", 4)]
        public static ConfigEntry<int> DeadlineDays => null;

        [Configuration<int>("Amount of credits everyone gets at the start of a new session (for vanilla, use: -1)",
            200)]
        public static ConfigEntry<int> StartingCredits => null;

        [Configuration<int>("Quota you begin with at the start of a new session (for vanilla, use: -1)", 400)]
        public static ConfigEntry<int> StartingQuota => null;

        [Configuration<int>("Rate at which the quota increases by when it's met (for vanilla, use: -1)", 275)]
        public static ConfigEntry<int> BaseIncrease => null;
    }

    public static void Bind(Plugin Plugin) {
        ConfigLoader.Bind(Plugin);
        // TODO: bind event config
        Validate();
    }

    private static void Validate() {
        LevelNames.AllCustom.ForEach(ConfigUtils.GetEnemyRarityValues);
        LevelNames.AllCustom.ForEach(ConfigUtils.GetScrapValues);
        ConfigUtils.GetMoonHeatCurve();
    }
}