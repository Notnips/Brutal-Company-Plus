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

    public void ExecuteServer(SelectableLevel Level) {
        // If there's more than one player;
        if (StartOfRound.Instance.allPlayerScripts.Length > 1)
            // pick a random player to be the target.
            _currentTarget = StartOfRound.Instance.allPlayerScripts.Random();
        else
            // Otherwise, this event does nothing, because it would just kill the only player.
            _playerKilled = true;
    }

    public void ExecuteClient(SelectableLevel Level) { }

    public void UpdateServer() {
        // If we've already killed a player, don't do anything.
        if (_playerKilled || _currentTarget == null) return;

        // If our current target died before we could kill them, pick a new target.
        if (_currentTarget.isPlayerDead && !StartOfRound.Instance.allPlayersDead) {
            _currentTarget = StartOfRound.Instance.allPlayerScripts.Random();
        }

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
}