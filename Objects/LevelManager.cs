using System;
using System.Collections.Generic;
using System.Linq;
using BrutalCompanyPlus.Utils;
using Unity.Netcode;
using UnityEngine;
using static BrutalCompanyPlus.Config.PluginConfig;

namespace BrutalCompanyPlus.Objects;

internal static class LevelManager {
    private static List<SpawnableEnemyWithRarity> _allEnemies;
    private static readonly Dictionary<string, Action> UndoPropertyCallbacks = new();

    /// <summary>
    /// Returns an immutable list of all enemies in the game.
    /// </summary>
    public static List<EnemyType> AllEnemies => _allEnemies.Select(Enemy => Enemy.enemyType).ToList();

    /// <summary>
    /// Returns an immutable list of all inside enemies in the game.
    /// </summary>
    public static List<EnemyType> AllInsideEnemies => _allEnemies.Where(Enemy => !Enemy.enemyType.isOutsideEnemy)
        .Select(Enemy => Enemy.enemyType).ToList();

    /// <summary>
    /// Returns an immutable list of all outside enemies in the game.
    /// </summary>
    public static List<EnemyType> AllOutsideEnemies => _allEnemies.Where(Enemy => Enemy.enemyType.isOutsideEnemy)
        .Select(Enemy => Enemy.enemyType).ToList();

    /// <summary>
    /// Returns an immutable list of all daytime enemies in the game.
    /// </summary>
    public static List<EnemyType> AllDaytimeEnemies => _allEnemies.Where(Enemy => Enemy.enemyType.isDaytimeEnemy)
        .Select(Enemy => Enemy.enemyType).ToList();


    /// <summary>
    /// Tries to get the <see cref="EnemyType"/> of the specified <see cref="EnemyAI"/> type.
    /// <para><b>This is an expensive operation and should not be called often.</b></para>
    /// </summary>
    /// <param name="EnemyType">the enemy type, if found</param>
    /// <typeparam name="T">type of the enemy</typeparam>
    /// <returns>true if the enemy was found</returns>
    public static bool TryGetEnemy<T>(out EnemyType EnemyType) where T : EnemyAI {
        EnemyType = _allEnemies.FirstOrDefault(Enemy => Enemy.enemyType.enemyPrefab.GetComponent<T>() != null)?
            .enemyType;
        return EnemyType != null;
    }

    /// <summary>
    /// Allows you to modify the properties of a level, and automatically undo the changes when the event ends.
    /// </summary>
    /// <param name="Level">the level you want to modify</param>
    /// <param name="Effect">the modification function</param>
    /// <param name="Dependencies">the names of the properties you've changed</param>
    public static void ModifyLevelProperties(SelectableLevel Level, Action<SelectableLevel> Effect,
        params string[] Dependencies) {
        // Store the original values of the properties we're about to change.
        var originalValues = new Dictionary<string, object>();
        foreach (var dependency in Dependencies) {
            var property = Level.GetType().GetField(dependency);
            if (!originalValues.TryAdd(dependency, property!.GetValue(Level)))
                throw new Exception($"Property '{dependency}' mentioned twice in dependencies array!");
        }

        // Apply the changes.
        Effect(Level);

        // Add a callback to undo the changes when the event ends.
        foreach (var dependency in Dependencies) {
            var property = Level.GetType().GetField(dependency);
            var originalValue = originalValues[dependency];
            var newValue = property!.GetValue(Level);
            if (originalValue.Equals(newValue)) continue;
            if (!UndoPropertyCallbacks.TryAdd(dependency, () => property.SetValue(Level, originalValue))) {
                throw new Exception(
                    $"Property '{dependency}' already contains an undo callback! " +
                    $"Did you call {nameof(ModifyLevelProperties)} twice on the same property?");
            }
        }
    }

    /// <summary>
    /// Spawns a map object in the level and syncs it to all clients.
    /// Also manages the object for you to automatically despawn it when the round ends.
    /// <b>You do not need to call <see cref="NetworkObject.Spawn"/> on the object.</b>
    /// </summary>
    /// <param name="Prefab">map object prefab, see <see cref="BcpUtils.FindObjectPrefab"/></param>
    /// <param name="Position">object position</param>
    /// <param name="Rotation">object rotation</param>
    public static void SpawnMapObject(GameObject Prefab, Vector3 Position, Quaternion Rotation) {
        UnityEngine.Object.Instantiate(Prefab, Position, Rotation, RoundManager.Instance.mapPropsContainer.transform)
            .GetComponent<NetworkObject>().Spawn(true);
    }

    /// <summary>
    /// Spawns a map object in the level and syncs it to all clients.
    /// Also manages the object for you to automatically despawn it when the round ends.
    /// <b>You do not need to call <see cref="NetworkObject.Spawn"/> on the object.</b>
    /// </summary>
    /// <param name="Prefab">map object prefab, see <see cref="BcpUtils.FindObjectPrefab"/></param>
    /// <param name="Position">object position</param>
    /// <param name="Rotation">object rotation</param>
    /// <typeparam name="T">behavior script of the map object</typeparam>
    /// <returns>the behavior script of the map object</returns>
    public static T SpawnMapObject<T>(GameObject Prefab, Vector3 Position, Quaternion Rotation) {
        var obj = UnityEngine.Object.Instantiate(Prefab, Position, Rotation,
            RoundManager.Instance.mapPropsContainer.transform);
        obj.GetComponent<NetworkObject>().Spawn(true);
        return obj.GetComponentInChildren<T>();
    }

    internal static void UndoLevelPropertyChanges() {
        Plugin.Logger.LogDebug("Undoing level property changes... (server)");
        foreach (var callback in UndoPropertyCallbacks.Values) callback();
        UndoPropertyCallbacks.Clear();
    }

    internal static void AddAllEnemiesToAllLevels(SelectableLevel[] Levels) {
        // NOTE: Make sure this code runs, even if SpawnOnAllMoons is disabled.
        // It's needed further down the line.
        _allEnemies = Levels.SelectMany(Level => Level.Enemies.Concat(Level.OutsideEnemies).Concat(Level.DaytimeEnemies))
            .GroupBy(Enemy => Enemy.enemyType.enemyName)
            .Select(Group => Group.First())
            .ToList();

        if (!EnemyAdjustments.SpawnOnAllMoons.Value) return;
        foreach (var level in Levels) {
            // Clear old enemy lists
            level.Enemies.Clear();
            level.OutsideEnemies.Clear();
            level.DaytimeEnemies.Clear();
            // Add all inside enemies to the level
            level.Enemies.AddRange(_allEnemies.Where(Enemy => !Enemy.enemyType.isOutsideEnemy));
            // Add all outside enemies to the level
            level.OutsideEnemies.AddRange(_allEnemies.Where(Enemy => Enemy.enemyType.isOutsideEnemy));
            // Add all daytime enemies to the level
            level.DaytimeEnemies.AddRange(_allEnemies.Where(Enemy => Enemy.enemyType.isDaytimeEnemy));
        }
    }

    internal static void ApplyEnemyRarityValues(SelectableLevel Level) {
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

    internal static void ApplyEnemySpawnRates(SelectableLevel Level) {
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

    internal static void ApplyLevelProperties(SelectableLevel Level) {
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