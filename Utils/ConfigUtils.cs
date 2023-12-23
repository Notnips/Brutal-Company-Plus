using System.Collections.Generic;
using BepInEx.Configuration;
using static BrutalCompanyPlus.Config.PluginConfig;

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

    internal static Dictionary<string, int> GetEnemyRarityValues(string LevelName) {
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

        var values = new Dictionary<string, int>();
        foreach (var entry in entries) {
            try {
                var (enemy, rarity) = ParseHelpers.ParseEnemyRarityEntry(entry);
                values.Add(enemy, rarity); // add parsed value to dictionary
            } catch (ParseException e) {
                Diagnostics.AddError($"Bad entry in enemy rarity values ({entry}): {e}");
            }
        }

        if (!values.IsEmpty()) return values; // values ok, return it
        Plugin.Logger.LogError($"Invalid enemy rarity values: {MoonHeat.HeatCurve.Value}");
        return values; // return empty dictionary
    }

    internal static (int, int, int, int) GetScrapValues(string LevelName) {
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
        }).Value.Trim();

        if (entries == ScrapValues.DefaultValues) {
            Plugin.Logger.LogWarning($"Using default scrap values for {LevelName}");
            return (-1, -1, -1, -1); // return default values
        }

        try {
            // return parsed values
            return ParseHelpers.ParseScrapValues(entries);
        } catch (ParseException e) {
            Diagnostics.AddError($"Invalid scrap values ({entries}): {e}");
            return (-1, -1, -1, -1); // return default values
        }
    }

    internal static List<(int, int, LevelWeatherType)> GetMoonHeatCurve() {
        var curve = new List<(int, int, LevelWeatherType)>();
        foreach (var point in MoonHeat.HeatCurve.Value.Split(',')) {
            try {
                var parsed = ParseHelpers.ParseHeatCurvePoint(point);
                curve.Add(parsed); // add parsed point to curve
            } catch (ParseException e) {
                Diagnostics.AddError($"Bad point in moon heat curve ({point}): {e}");
            }
        }

        if (!curve.IsEmpty()) return curve; // curve ok, return it
        Plugin.Logger.LogError($"Invalid moon heat curve: {MoonHeat.HeatCurve.Value}");
        Plugin.Logger.LogError("Using fallback curve instead (0-100%: no weather)");
        curve.Add((0, /* exclusive */ 101, LevelWeatherType.None)); // default curve
        return curve; // return fallback curve
    }
}