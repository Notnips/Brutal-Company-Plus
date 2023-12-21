using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using static BrutalCompanyPlus.PluginConfig;

namespace BrutalCompanyPlus.Utils;

public static class ConfigUtils {
    public static bool GetIfSet<T>(this ConfigEntry<T> Entry, out T Value) where T : notnull {
        if (Entry.Value is -1) {
            Value = default;
            return false;
        }

        Value = Entry.Value;
        return true;
    }

    public static Dictionary<string, int> GetEnemyRarityValues(string LevelName) {
        var entries = (LevelName switch {
            LevelNames.Experimentation => EnemyRarityValues.ExperimentationLevel,
            LevelNames.Assurance => EnemyRarityValues.AssuranceLevel,
            LevelNames.Vow => EnemyRarityValues.VowLevel,
            LevelNames.Offense => EnemyRarityValues.OffenseLevel,
            LevelNames.March => EnemyRarityValues.MarchLevel,
            LevelNames.Rend => EnemyRarityValues.RendLevel,
            LevelNames.Dine => EnemyRarityValues.DineLevel,
            LevelNames.Titan => EnemyRarityValues.TitanLevel,
            _ => EnemyRarityValues.CustomLevel
        }).Value.Split(',');

        var rarityValues = new Dictionary<string, int>();
        var error = false;
        foreach (var entry in entries) {
            var values = entry.Split(':');
            if (values.Length != 2) error = true;
            var enemy = values[0].Trim(); // TODO: check if enemy exists
            if (!int.TryParse(values[1], out var rarity)) error = true;
            if (error) {
                Diagnostics.AddError($"Bad entry in enemy rarity values: {entry}");
                error = false;
                continue;
            }

            rarityValues.Add(enemy, rarity);
        }

        if (!rarityValues.IsEmpty()) return rarityValues; // values ok, return it
        Plugin.Logger.LogError($"Invalid enemy rarity values: {MoonHeat.HeatCurve.Value}");
        return rarityValues; // return empty dictionary
    }

    public static (int, int, int, int) GetScrapValues(string LevelName) {
        var entries = (LevelName switch {
            LevelNames.Experimentation => ScrapValues.ExperimentationLevel,
            LevelNames.Assurance => ScrapValues.AssuranceLevel,
            LevelNames.Vow => ScrapValues.VowLevel,
            LevelNames.Offense => ScrapValues.OffenseLevel,
            LevelNames.March => ScrapValues.MarchLevel,
            LevelNames.Rend => ScrapValues.RendLevel,
            LevelNames.Dine => ScrapValues.DineLevel,
            LevelNames.Titan => ScrapValues.TitanLevel,
            _ => ScrapValues.CustomLevel
        }).Value;
        if (entries == ScrapValues.DefaultValues) {
            Plugin.Logger.LogWarning($"Using default scrap values for {LevelName}");
            goto ReturnDefaults;
        }

        var values = entries.Split(',');
        if (values.Length != 4) {
            var msg = $"Invalid scrap values: {entries}";
            Plugin.Logger.LogError(msg);
            Diagnostics.AddError(msg);
            goto ReturnDefaults;
        }

        if (!int.TryParse(values[0], out var a)) goto ReturnDefaults;
        if (!int.TryParse(values[1], out var b)) goto ReturnDefaults;
        if (!int.TryParse(values[2], out var c)) goto ReturnDefaults;
        if (!int.TryParse(values[3], out var d)) goto ReturnDefaults;
        return (a, b, c, d); // return parsed values

        ReturnDefaults:
        return (-1, -1, -1, -1); // default values
    }

    public static List<(int, int, LevelWeatherType)> GetMoonHeatCurve() {
        var curve = new List<(int, int, LevelWeatherType)>();
        var error = false;
        foreach (var point in MoonHeat.HeatCurve.Value.Split(',')) {
            var values = point.Split(':');
            if (values.Length != 3) error = true;
            if (!int.TryParse(values[0], out var start)) error = true;
            if (!int.TryParse(values[1], out var end)) error = true;
            if (!Enum.TryParse(values[2], out LevelWeatherType type)) error = true;
            if (error) {
                Diagnostics.AddError($"Bad point in moon heat curve: {point}");
                error = false;
                continue;
            }

            curve.Add((start, end, type)); // add parsed point to curve
        }

        if (!curve.IsEmpty()) return curve; // curve ok, return it
        Plugin.Logger.LogError($"Invalid moon heat curve: {MoonHeat.HeatCurve.Value}");
        Plugin.Logger.LogError("Using fallback curve instead (0-100%: no weather)");
        curve.Add((0, 100, LevelWeatherType.None)); // default curve
        return curve; // return fallback curve
    }
}