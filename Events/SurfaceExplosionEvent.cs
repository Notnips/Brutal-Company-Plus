// ReSharper disable InconsistentNaming

using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using BrutalCompanyPlus.Utils;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
[HarmonyPatch]
public class SurfaceExplosionEvent : IEvent {
    private static readonly Vector3 MineDistance = new(9.33f, 5.2f, 1021f);

    // speed at which mines spawn
    private const int StartSpawnInterval = 10; // in seconds
    private const int MinSpawnInterval = 1; // in seconds
    private const int MaxSpawnInterval = 4; // in seconds

    private GameObject _prefab;
    private float _spawnTimer;

    public string Name => "The surface is explosive!";
    public string Description => "EXPLOSION!!!";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Uncommon;

    public void ExecuteServer(SelectableLevel Level) {
        _spawnTimer = StartSpawnInterval;
        _prefab ??= Level.FindObjectPrefab<Landmine>();
    }

    public void ExecuteClient(SelectableLevel Level) { }

    public void UpdateServer() {
        if (StartOfRound.Instance.allPlayersDead) return;
        if (_spawnTimer >= 0f) {
            _spawnTimer -= Time.deltaTime;
            return;
        }

        _spawnTimer = Random.Range(MinSpawnInterval, MaxSpawnInterval);
        if (!PlayerUtils.OutsidePlayers.Random(out var player)) return;
        var playerPos = player.transform.position;
        if (Vector3.Distance(playerPos, MineDistance) < 1f) {
            _spawnTimer = -1f;
            return;
        }

        // We trigger the mine immediately to prevent a client-advantage where the mine
        // does not trigger because the client has already moved too far away from the mine.
        LevelManager.SpawnMapObject<Landmine>(_prefab, playerPos, Quaternion.identity)
            .TriggerMineOnLocalClientByExiting(); // trigger the mine immediately
    }

    [HarmonyPrefix, HarmonyPatch(typeof(Landmine), "Start")]
    private static bool StartPatch(ref Landmine __instance) {
        // If the mine was already triggered (possibly by the function above),
        // we don't want the animation to reset back to the idle state.
        return !__instance.hasExploded;
    }
}