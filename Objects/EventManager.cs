using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Events;
using BrutalCompanyPlus.Utils;
using static BrutalCompanyPlus.Config.PluginConfig;

namespace BrutalCompanyPlus.Objects;

public static class EventManager {
    internal static IEvent CurrentEvent;

    public static bool IsActive<T>() where T : IEvent => CurrentEvent is T;

    public static void StartEventServer(SelectableLevel Level) {
        var @event = SelectRandomEvent(Level);
        if (@event == null) return;
        Plugin.Logger.LogWarning($"Starting event {@event.Name}... (server)");
        BCNetworkManager.Instance.StartEventClientRpc(@event.GetId(), Level.levelID);
        @event.ExecuteServer(Level);
        NotifyEventStarted(@event);
        CurrentEvent = @event;
    }

    public static void EndEventServer(SelectableLevel Level) {
        Plugin.Logger.LogWarning($"Ending event {CurrentEvent.Name}... (server)");
        BCNetworkManager.Instance.EndEventClientRpc();
        CurrentEvent.OnEnd(Level);
        LevelManager.UndoLevelPropertyChanges();
        CurrentEvent = null;
    }

    public static void StartEventClient(SelectableLevel Level, IEvent Event) {
        Plugin.Logger.LogWarning($"Starting event {Event.Name}... (client)");
        Event.ExecuteClient(Level);
        CurrentEvent = Event;
    }

    public static void EndEventClient(SelectableLevel Level) {
        Plugin.Logger.LogWarning($"Ending event {CurrentEvent.Name}... (client)");
        CurrentEvent.OnEnd(Level);
        CurrentEvent = null;
    }

    private static IEvent SelectRandomEvent(SelectableLevel Level) {
        if (Level.sceneName == "CompanyBuilding") {
            Plugin.Logger.LogWarning("Landed at The Company Building, forcing no event...");
            return EventRegistry.GetEvent<NoneEvent>();
        }

        const int maxAttempts = 20;
        var attempts = 0;
        while (true) {
            var chance = UnityEngine.Random.Range(0, /* exclusive */ 101);
            // Roll a dice to see if an event should happen at all.
            var @event = chance > EventSettings.GlobalChance.Value
                // Random chance exceeded configured chance, so no event.
                ? EventRegistry.GetEvent<NoneEvent>()
                // If all events are equally likely to happen;
                : EventSettings.EqualChance.Value
                    // just return a random event.
                    ? EventRegistry.GetRandomEventWithoutRarity()
                    // Otherwise, return a random event based on their rarity.
                    : EventRegistry.GetRandomEvent();

            // If the event can run on the current level, return it.
            if (@event.CanRun(Level)) return @event;
            // Otherwise, try again up to the maximum amount of attempts.
            if (++attempts > maxAttempts) {
                Plugin.Logger.LogWarning(
                    $"Failed to select an event ({attempts} > {maxAttempts}), forcing no event...");
                return EventRegistry.GetEvent<NoneEvent>();
            }

            // Log the failed attempt.
            Plugin.Logger.LogWarning(
                $"Event {@event.Name} cannot be ran, rerolling... (attempt {attempts} / {maxAttempts})");
        }
    }

    private static void NotifyEventStarted(IEvent Event) {
        var positivity = Event.Positivity switch {
            EventPositivity.Positive => "green",
            EventPositivity.Neutral => "white",
            EventPositivity.Negative => "red",
            EventPositivity.Golden => "orange",
            _ => "white"
        };
        ChatUtils.Send(
            $"<color=yellow>EVENT<color=white>:</color></color>\n" +
            $"<color={positivity}>{Event.Name}</color>\n" +
            $"<color=white><size=70%>{Event.Description}</size></color>"
        );
    }
}