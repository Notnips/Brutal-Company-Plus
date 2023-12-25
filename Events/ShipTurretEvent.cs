using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Utils;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
public class ShipTurretEvent : IEvent {
    private static readonly Vector3 TurretPosition = new(3.87f, 0.84f, -14.23f);

    private GameObject _prefab;
    private GameObject _turret;
    private bool _shouldSpawn;
    private bool _spawned;

    public string Name => "When did we get this installed?!";
    public string Description => "I'm innocent, I swear!";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Uncommon;

    public void ExecuteServer(SelectableLevel Level) {
        _prefab ??= Level.FindObjectPrefab<Turret>();
        _shouldSpawn = true;
    }

    public void ExecuteClient(SelectableLevel Level) { }

    public void UpdateServer() {
        if (_shouldSpawn) {
            _shouldSpawn = false;
            _spawned = true;
            _turret = Object.Instantiate(_prefab, TurretPosition, Quaternion.identity);
            _turret.transform.forward = new Vector3(1f, 0f, 0f);
            _turret.GetComponent<NetworkObject>().Spawn(true);
        } else if (_spawned && StartOfRound.Instance.shipIsLeaving) {
            _spawned = false;
            _turret.GetComponent<NetworkObject>().Despawn();
            ChatUtils.Send("<color=green>Turret removal procedure complete.</color>");
        }
    }

    public void OnEnd(SelectableLevel Level) {
        _shouldSpawn = false;
        _spawned = false;
    }
}