using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Utils;
using GameNetcodeStuff;
using JetBrains.Annotations;
using UnityEngine;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
public class HungerGamesEvent : IEvent {
    private PlayerControllerB _currentTarget;
    private bool _playerKilled;

    public string Name => "The Hunger Games";
    public string Description => "May the odds be ever in your favor.";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Rare;
    public bool CanRun(SelectableLevel Level) => StartOfRound.Instance.connectedPlayersAmount > 0; // 0 is the host.

    public void ExecuteServer(SelectableLevel Level) {
        // Pick a random player to be the target.
        PickNewTarget();
    }

    public void ExecuteClient(SelectableLevel Level) { }

    public void UpdateServer() {
        // If we've already killed a player, don't do anything.
        if (_playerKilled || _currentTarget == null) return;

        // If our current target died before we could kill them;
        if (_currentTarget.isPlayerDead && !StartOfRound.Instance.allPlayersDead)
            PickNewTarget(); // pick a new target.

        // If our current target has just entered the factory;
        if (!_currentTarget.isInsideFactory) return;

        // Kill them.
        _playerKilled = true;
        _currentTarget.DamagePlayerFromOtherClientServerRpc(
            _currentTarget.health, new Vector3(),
            (int)_currentTarget.playerClientId);

        // Notify the players that the target has been killed.
        ChatUtils.Send($"<color=purple>{_currentTarget.playerUsername}</color> " +
                       $"<color=orange>volunteered as Tribute!</color>");
    }

    public void OnEnd(SelectableLevel Level) {
        _currentTarget = null;
        _playerKilled = false;
    }

    private void PickNewTarget() {
        // Pick a random player to be the target.
        if (!PlayerUtils.AlivePlayers.Random(out _currentTarget)) return;
        this.Log($"New target chosen: {_currentTarget.playerUsername}");
    }
}