using System.Linq;
using BrutalCompanyPlus.Utils;
using UnityEngine;
using static BrutalCompanyPlus.PluginConfig;

namespace BrutalCompanyPlus.Objects;

public static class LevelManager {
    public static void AddAllEnemiesToAllLevels() {
        if (!EnemyAdjustments.SpawnOnAllMoons.Value) return;

        var levels = StartOfRound.Instance.levels;
        var enemies = levels.SelectMany(Level => Level.Enemies)
            .GroupBy(Enemy => Enemy.enemyType.enemyName)
            .Select(Group => Group.First())
            .ToList();

        foreach (var level in levels) {
            level.Enemies.Clear();
            level.Enemies.AddRange(enemies);
        }
    }

    public static void ApplyEnemyRarityValues(SelectableLevel Level) {
        if (!EnemyRarityValues.Enabled.Value) return;
        var values = ConfigUtils.GetEnemyRarityValues(Level.name);
        var levelName = Level.name;
        if (LevelNames.IsCustom(levelName)) levelName = LevelNames.Custom;
        if (!LevelDefaults.DefaultEnemyRarityValues.TryGetValue(levelName, out var defaults)) {
            Plugin.Logger.LogError($"No default rarity values found for level: {Level.name}");
            return;
        }

        foreach (var enemy in Level.Enemies) {
            var name = enemy.enemyType.enemyName;
            if (!values.TryGetValue(name, out var rarity) || rarity == -1) {
                if (!defaults.TryGetValue(name, out rarity)) {
                    Plugin.Logger.LogError($"No default rarity value found for enemy: {name}");
                    continue;
                }
            }

            // set enemy rarity
            enemy.rarity = rarity;
        }
    }

    public static void ApplyEnemySpawnRates(SelectableLevel Level) {
        Level.enemySpawnChanceThroughoutDay = new AnimationCurve(
            new Keyframe(0f, 0.1f),
            new Keyframe(0.5f, 10f),
            new Keyframe(1f, 70f)
        );
        Level.outsideEnemySpawnChanceThroughDay = new AnimationCurve(
            new Keyframe(0f, -30f),
            new Keyframe(20f, -20f),
            new Keyframe(21f, 10f)
        );

        foreach (var mapObject in Level.spawnableMapObjects) {
            if (mapObject.IsObjectTypeOf<Turret>(out _) && MapHazards.TurretSpawnRate.GetIfSet(out var rate)) {
                mapObject.numberToSpawn = new AnimationCurve(
                    new Keyframe(0f, 0f),
                    new Keyframe(1f, rate)
                );
            }

            if (mapObject.IsObjectTypeOf<Landmine>(out _) && MapHazards.LandmineSpawnRate.GetIfSet(out rate)) {
                mapObject.numberToSpawn = new AnimationCurve(
                    new Keyframe(0f, 0f),
                    new Keyframe(1f, rate)
                );
            }
        }
    }

    public static void ApplyLevelProperties(SelectableLevel Level) {
        var (min, max, minTotal, maxTotal) = ConfigUtils.GetScrapValues(Level.name);

        ApplyIfSet(ref Level.minScrap, min);
        ApplyIfSet(ref Level.maxScrap, max);
        ApplyIfSet(ref Level.minTotalScrapValue, minTotal);
        ApplyIfSet(ref Level.maxTotalScrapValue, maxTotal);

        Level.maxEnemyPowerCount += 150;
        Level.maxOutsideEnemyPowerCount += 10;
        Level.maxDaytimeEnemyPowerCount += 150;
    }

    private static void ApplyIfSet(ref int Value, int NewValue) {
        if (NewValue != -1) Value = NewValue;
    }
}