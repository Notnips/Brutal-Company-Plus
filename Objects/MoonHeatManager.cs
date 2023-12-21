using System.Collections.Generic;
using System.Linq;
using DunGen;
using UnityEngine;
using BrutalCompanyPlus.Utils;
using static BrutalCompanyPlus.PluginConfig;

namespace BrutalCompanyPlus.Objects;

public static class MoonHeatManager {
    private static readonly IntRange MoonHeatRange = new(0, 100);

    private static readonly Dictionary<SelectableLevel, float> LevelHeatValues = new();
    private static readonly Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>> LevelEnemies = new();

    public static void AdjustHeatValues(SelectableLevel CurrentLevel) {
        // Initialize moon heat values if they haven't been already
        InitializeFor(CurrentLevel);

        // Increase the heat of the current level and store the previous value
        var heat = IncreaseHeatValueOf(CurrentLevel);

        // Decrease heat for all levels except the current one
        foreach (var level in LevelHeatValues.Keys) {
            if (CurrentLevel == level) continue;
            DecreaseHeatValueOf(level);
        }

        // Notify players of the current moon's heat
        NotifyPlayersAndSetWeather(CurrentLevel, heat);
    }

    private static void InitializeFor(SelectableLevel NewLevel) {
        if (!LevelHeatValues.TryAdd(NewLevel, 0f)) return;
        LevelEnemies.TryAdd(NewLevel, NewLevel.Enemies.ToList());
    }

    private static float IncreaseHeatValueOf(SelectableLevel Level) {
        if (!LevelHeatValues.TryGetValue(Level, out var heat)) return 0f;
        LevelHeatValues[Level] = Mathf.Clamp(
            heat + MoonHeat.IncreaseRate.Value,
            MoonHeatRange.Min, MoonHeatRange.Max
        );
        return heat;
    }

    private static void DecreaseHeatValueOf(SelectableLevel Level) {
        if (!LevelHeatValues.TryGetValue(Level, out var heat)) return;
        LevelHeatValues[Level] = Mathf.Clamp(
            heat - MoonHeat.DecreaseRate.Value,
            MoonHeatRange.Min, MoonHeatRange.Max
        );
    }

    private static void NotifyPlayersAndSetWeather(SelectableLevel CurrentLevel, float Heat) {
        ChatUtils.Send("<color=orange>MOON IS AT " + Heat + "% HEAT</color>", Clear: true);
        foreach (var (start, end, type) in ConfigUtils.GetMoonHeatCurve()) {
            if (Heat < start || Heat >= end) continue;
            CurrentLevel.currentWeather = type; // TODO: network this
            NotifyChat(type switch {
                LevelWeatherType.Rainy => "blue",
                LevelWeatherType.Foggy => "purple",
                LevelWeatherType.Flooded => "yellow",
                LevelWeatherType.Stormy => "orange",
                LevelWeatherType.Eclipsed => "red",
                _ => "white"
            }, type switch {
                LevelWeatherType.Rainy => "Heat is rising and caused it to rain...",
                LevelWeatherType.Foggy => "Heat is rising, causing a layer of fog...",
                LevelWeatherType.Flooded => "Moon is getting hot, causing a flood...",
                LevelWeatherType.Stormy => "Extreme heat is causing a dangerous weather...",
                LevelWeatherType.Eclipsed => "Moon is at max heat, causing hostile creatures to roam it...",
                _ => "Heat is rising..."
            });
            break;
        }
    }

    private static void NotifyChat(string Color, string Title) => ChatUtils.Send(
        "<size=11>" +
        $"<color={Color}>{Title}</color>\n" +
        "<color=white>Visit other moons to decrease the heat!</color>" +
        "</size>"
    );
}