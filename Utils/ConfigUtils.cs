using BepInEx.Configuration;

namespace BrutalCompanyPlus.Utils;

public static class ConfigUtils {
    public static ConfigEntry<string> GetConfigForLevel(string LevelName) =>
        LevelName switch {
            "ExperimentationLevel" => PluginConfig.ExperimentationLevelRarities,
            "AssuranceLevel" => PluginConfig.AssuranceLevelRarities,
            "VowLevel" => PluginConfig.VowLevelRarities,
            "MarchLevel" => PluginConfig.MarchLevelRarities,
            "RendLevel" => PluginConfig.RendLevelRarities,
            "DineLevel" => PluginConfig.DineLevelRarities,
            "OffenseLevel" => PluginConfig.OffenseLevelRarities,
            "TitanLevel" => PluginConfig.TitanLevelRarities,
            _ => null
        };
}