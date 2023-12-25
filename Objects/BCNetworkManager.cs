// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault

using System.Linq;
using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Utils;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyPlus.Objects;

public class BCNetworkManager : NetworkBehaviour {
    private const string Tag = $"[{nameof(BCNetworkManager)}]";

    private const string DisconnectMessage = $"{PluginInfo.PLUGIN_NAME}\n" +
                                             $"Server sent invalid enemy reference!\n" +
                                             $"This is a bug, please report it!";

    public static BCNetworkManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
        Log($"{nameof(BCNetworkManager)} initialized!");
    }

    private void Update() {
        if (!IsServer || !IsHost) return; // only host should execute this
        if (EventManager.CurrentEvent == null) return; // no event is active
        EventManager.CurrentEvent.UpdateServer();
    }

    [ClientRpc]
    public void SyncEnemyTypeClientRpc(NetworkBehaviourReference Reference, bool IsOutside) {
        if (IsServer || IsHost) return; // only clients should execute this
        Log($"Syncing enemy type (outside: {IsOutside})... (client)");

        // Get the enemy AI component
        if (!Reference.TryGet(out EnemyAI enemy)) {
            Log("Bad enemy received from server! Disconnecting client to prevent further issues... (client)");
            GameNetworkManager.Instance.disconnectionReasonMessage = DisconnectMessage;
            GameNetworkManager.Instance.Disconnect();
            return;
        }

        // Sync the outside enemy flag.
        enemy.enemyType = EnemyUtils.SetOutsideEnemy(enemy.enemyType, IsOutside);

        // We can figure out if Start has already been called by checking if path1 is initialized.
        // If it is, we need to call EnableEnemyMesh to ensure the enemy is visible.
        if (enemy.path1 != null && IsOutside && GameNetworkManager.Instance.localPlayerController != null) {
            Plugin.Logger.LogWarning($"Received SyncEnemyType RPC for {enemy.name} too late, catching up...");
            enemy.EnableEnemyMesh(!StartOfRound.Instance.hangarDoorsClosed ||
                                  !GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom);
        }

        // We might be too late at this point (as mentioned above),
        // so set isOutside and allAINodes as well, just in case.
        enemy.isOutside = IsOutside;
        enemy.allAINodes = GameObject.FindGameObjectsWithTag(IsOutside ? "OutsideAINode" : "AINode");
    }

    [ClientRpc]
    public void StartEventClientRpc(int EventId, int LevelId) {
        if (IsServer || IsHost) return; // only clients should execute this
        Log($"Received start event request: eid = {EventId}, lid = {LevelId}");

        // Parse event id
        var @event = EventRegistry.GetEvent(EventId);
        if (@event == null) {
            Log($"Bad event id {EventId} received from server.");
            return;
        }

        // Parse level id
        var level = StartOfRound.Instance.levels.ElementAtOrDefault(LevelId);
        if (level == null) {
            Log($"Bad level id {LevelId} received from server.");
            return;
        }

        // Execute event
        EventManager.StartEventClient(level, @event);
    }

    [ClientRpc]
    public void EndEventClientRpc() {
        if (IsServer || IsHost) return; // only clients should execute this
        Log("Received end event request... (client)");
        EventManager.EndEventClient(RoundManager.Instance.currentLevel);
    }

    [ClientRpc]
    public void SyncWeatherClientRpc(int LevelId, LevelWeatherType WeatherType) {
        Log($"Syncing weather type {WeatherType} on level {LevelId}...");

        // Parse level id
        var level = StartOfRound.Instance.levels.ElementAtOrDefault(LevelId);
        if (level == null) {
            Log($"Bad level id {LevelId} received from server.");
            return;
        }

        // Sync weather
        level.currentWeather = WeatherType;
    }

    private static void Log(string Message) => Plugin.Logger.LogWarning($"{Tag} {Message}");
}