using System;
using System.Collections.Generic;

namespace BrutalCompanyPlus.Objects;

public static class EnemySpawnManager {
    internal static readonly List<(EnemyType EnemyType, SpawnInfo SpawnInfo)> PendingSpawns = new();

    /// <summary>
    /// Drafts the specified amount of enemies to spawn when the level starts.
    /// </summary>
    /// <param name="Info">spawn info</param>
    /// <typeparam name="T">type of the enemy</typeparam>
    public static void DraftEnemySpawn<T>(SpawnInfo Info) where T : EnemyAI {
        if (!LevelManager.TryGetEnemy<T>(out var type))
            throw new ArgumentException($"Failed to get EnemyType for {typeof(T).Name}!");
        PendingSpawns.Add((type, Info));
    }

    /// <summary>
    /// Drafts the specified amount of enemies to spawn when the level starts.
    /// </summary>
    /// <param name="Type">type of the enemy</param>
    /// <param name="Info">spawn info</param>
    public static void DraftEnemySpawn(EnemyType Type, SpawnInfo Info) {
        PendingSpawns.Add((Type, Info));
    }

    public readonly struct SpawnInfo {
        /// <summary>
        /// Amount of enemies to spawn.
        /// </summary>
        public readonly int Amount;

        /// <summary>
        /// Whether the enemies should be spawned inside or outside.
        /// </summary>
        public readonly bool Outside;

        /// <summary>
        /// Whether the enemy should be spawned immediately, or after at least one player has entered the facility.
        /// </summary>
        public readonly bool Immediate;

        /// <summary>
        /// Creates a new SpawnInfo instance.
        /// </summary>
        /// <param name="Amount">Amount of enemies to spawn.</param>
        /// <param name="Outside">Whether the enemies should be spawned inside or outside.</param>
        /// <param name="Immediate">Whether the enemy should be spawned immediately, or after at least one player has entered the facility.</param>
        public SpawnInfo(int Amount, bool Outside = false, bool Immediate = true) {
            this.Amount = Amount;
            this.Outside = Outside;
            this.Immediate = Immediate;
        }
    }
}