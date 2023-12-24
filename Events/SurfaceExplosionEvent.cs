// ReSharper disable InconsistentNaming

using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Utils;
using HarmonyLib;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
[HarmonyPatch]
public class SurfaceExplosionEvent : IEvent {
    private static readonly Vector3 MineDistance = new(9.33f, 5.2f, 1021f);

    // speed at which mines spawn
    private const int MinSpawnInterval = 1; // in seconds
    private const int MaxSpawnInterval = 4; // in seconds

    private GameObject _prefab;
    private float _spawnTimer;

    public string Name => "The surface is explosive!";
    public string Description => "EXPLOSION!!!";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Uncommon;

    public void ExecuteServer(SelectableLevel Level) {
        _prefab ??= Level.FindObjectPrefab<Landmine>();
    }

    public void ExecuteClient(SelectableLevel Level) { }

    public void UpdateServer() {
        if (StartOfRound.Instance.allPlayersDead) return;
        if (_spawnTimer >= 0.0) {
            _spawnTimer -= Time.deltaTime;
            return;
        }

        _spawnTimer = Random.Range(MinSpawnInterval, MaxSpawnInterval);
        var player = StartOfRound.Instance.allPlayerScripts.Random();
        if (player.isInHangarShipRoom || player.isInsideFactory) return;
        if (Vector3.Distance(player.transform.position, MineDistance) < 1f) {
            _spawnTimer = MinSpawnInterval;
            return;
        }

        var mine = Object.Instantiate(_prefab, player.transform.position, Quaternion.identity);
        mine.GetComponent<NetworkObject>().Spawn(true);
    }

    public void OnEnd(SelectableLevel Level) {
        _spawnTimer = 0f;
    }
}