using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Events;
using static BrutalCompanyPlus.Config.PluginConfig;

namespace BrutalCompanyPlus.Objects;

public static class EventManager {
    private static IEvent _currentEvent;

    public static void StartEventServer(SelectableLevel Level) {
        var @event = SelectRandomEvent(Level);
        if (@event == null) return;
        Plugin.Logger.LogWarning($"Starting event {@event.Name}... (server)");
        BCNetworkManager.Instance.StartEventClientRpc(@event.GetId(), Level.levelID);
        @event.ExecuteServer(Level);
        NotifyEventStarted(@event);
        _currentEvent = @event;
    }

    public static void EndEventServer(SelectableLevel Level) {
        Plugin.Logger.LogWarning($"Ending event {_currentEvent.Name}... (server)");
        BCNetworkManager.Instance.EndEventClientRpc();
        _currentEvent.OnEnd(Level);
        _currentEvent = null;
    }

    public static void StartEventClient(SelectableLevel Level, IEvent Event) {
        Plugin.Logger.LogWarning($"Starting event {Event.Name}... (client)");
        Event.ExecuteClient(Level);
        _currentEvent = Event;
    }

    public static void EndEventClient(SelectableLevel Level) {
        Plugin.Logger.LogWarning($"Ending event {_currentEvent.Name}... (client)");
        _currentEvent.OnEnd(Level);
        _currentEvent = null;
    }

    private static IEvent SelectRandomEvent(SelectableLevel Level) {
        if (Level.sceneName == "CompanyBuilding") {
            Plugin.Logger.LogWarning("Landed at The Company Building, forcing no event...");
            return EventRegistry.GetEvent<NoneEvent>();
        }

        var chance = UnityEngine.Random.Range(0, /* exclusive */ 101);
        return chance <= EventSettings.GlobalChance.Value
            ? EventRegistry.GetEvent<NoneEvent>()
            : EventRegistry.GetRandomEvent();
    }

    private static void NotifyEventStarted(IEvent Event) {
        var positivity = Event.Positivity switch {
            EventPositivity.Positive => "green",
            EventPositivity.Neutral => "white",
            EventPositivity.Negative => "red",
            EventPositivity.Golden => "orange",
            _ => "white"
        };
        HUDManager.Instance.AddTextToChatOnServer(
            $"<color=yellow>EVENT<color=white>:</color></color>\n" +
            $"<color={positivity}>{Event.Name}</color>\n" +
            $"<color=white><size=70%>{Event.Description}</size></color>"
        );
    }
}