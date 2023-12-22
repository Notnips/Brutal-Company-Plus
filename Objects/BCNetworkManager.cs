// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault

using System.Linq;
using BrutalCompanyPlus.Api;
using Unity.Netcode;

namespace BrutalCompanyPlus.Objects;

public class BCNetworkManager : NetworkBehaviour {
    private const string Tag = $"[{nameof(BCNetworkManager)}]";

    public static BCNetworkManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
        Log($"{nameof(BCNetworkManager)} initialized!");
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

    private static void Log(string Message) => Plugin.Logger.LogWarning($"{Tag} {Message}");
}