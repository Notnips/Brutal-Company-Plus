using System.Collections.Generic;
using System.Linq;
using BrutalCompanyPlus.Utils;
using DunGen;
using UnityEngine;

namespace BrutalCompanyPlus.Objects;

public class MoonHeatManager {
    private static readonly IntRange MoonHeatRange = new(0, 100);

    private static readonly Dictionary<SelectableLevel, float> LevelHeatValues = new();
    private static readonly Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>> LevelEnemies = new();

    public static void InitializeLevel(SelectableLevel NewLevel) {
        LevelHeatValues.TryAdd(NewLevel, 0f);
        LevelEnemies.TryAdd(NewLevel, NewLevel.Enemies.ToList());
    }

    public static void DecreaseHeatValue(SelectableLevel CurrentLevel) {
        if (!LevelHeatValues.TryGetValue(CurrentLevel, out var heat)) return;
        LevelHeatValues[CurrentLevel] = Mathf.Clamp(
            heat + PluginConfig.MoonHeatIncreaseRate.Value,
            MoonHeatRange.Min, MoonHeatRange.Max
        );
    }

    public static void DecreaseHeatValues(SelectableLevel CurrentLevel) {
        foreach (var (level, heat) in LevelHeatValues) {
            if (CurrentLevel == level) continue;
            LevelHeatValues[level] = Mathf.Clamp(
                heat - PluginConfig.MoonHeatDecreaseRate.Value,
                MoonHeatRange.Min, MoonHeatRange.Max
            );
        }
    }

    public static void NotifyPlayers(SelectableLevel CurrentLevel) {
        if (!LevelHeatValues.TryGetValue(CurrentLevel, out var heat)) return;
        ChatUtils.Send("<color=orange>MOON IS AT " + heat + "% HEAT</color>", Clear: true);
        ChatUtils.SendIf(heat >= 30f, () =>
            "<size=10>" +
            "<color=red>MOON IS GETTING HOT</color>\n" +
            "<color=white>Visit other moons to decrease your moon heat!</color>" +
            "</size>");
    }
}