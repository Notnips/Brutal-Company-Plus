using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BrutalCompanyPlus.Config;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Api;

public static class EventRegistry {
    private static readonly List<IEvent> RegisteredEvents = new();

    [UsedImplicitly]
    public static void RegisterEvents(BaseUnityPlugin FromPlugin, List<IEvent> Events) {
        EventConfig.RegisterWith(FromPlugin, ref Events);
        RegisteredEvents.AddRange(Events);

        var pluginName = FromPlugin.Info.Metadata.Name;
        Plugin.Logger.LogWarning($"Registered {Events.Count} events from {pluginName}...");
        foreach (var @event in Events) {
            Plugin.Logger.LogWarning(
                $"Registered event {@event.Name} (rarity: {@event.Rarity}, from: {pluginName})...");
        }
    }

    public static void AutoRegister(BaseUnityPlugin FromPlugin) {
        List<IEvent> events = new();
        foreach (var type in Assembly.GetCallingAssembly().GetTypes()) {
            if (!typeof(IEvent).IsAssignableFrom(type)) return;
            var @event = (IEvent)type.GetConstructors()[0].Invoke(null);
            events.Add(@event);
        }

        // Register the events
        RegisterEvents(FromPlugin, events);
    }
}