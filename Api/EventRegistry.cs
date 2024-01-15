using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BrutalCompanyPlus.Config;
using BrutalCompanyPlus.Events;
using BrutalCompanyPlus.Objects;
using BrutalCompanyPlus.Utils;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Api;

#pragma warning disable CS0618 // Type or member is obsolete
public static class EventRegistry {
    private static readonly List<IEvent> RegisteredEvents = new();
    internal static readonly Dictionary<IEvent, EventRarity> EventRarityValues = new();

    [UsedImplicitly]
    public static void RegisterEvents(BaseUnityPlugin FromPlugin, List<IEvent> Events) {
        EventConfig.RegisterWith(FromPlugin, ref Events);
        RegisteredEvents.AddRange(Events);

        Plugin.Logger.LogWarning($"Registered {Events.Count} events from {FromPlugin.Info.Metadata.Name}:");
        foreach (var @event in Events) {
            Plugin.Logger.LogWarning(
                $"Registered event: {@event.Name} (rarity: {@event.GetRarity()}, description: {@event.Description})");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void AutoRegister(BaseUnityPlugin FromPlugin) {
        // Get all events from the calling assembly
        var events = from type in Assembly.GetCallingAssembly().GetTypes()
            where type.IsClass && typeof(IEvent).IsAssignableFrom(type)
            select (IEvent)Activator.CreateInstance(type);

        // Register the events
        RegisterEvents(FromPlugin, events.ToList());
    }

    public static T GetEvent<T>() where T : IEvent =>
        (T)(RegisteredEvents.FirstOrDefault(Event => Event is T) ?? default(T));

    public static IEvent GetEvent(int EventId) => RegisteredEvents.ElementAtOrDefault(EventId);

    private static EventRarity GetRarity(this IEvent Event) => EventRarityValues[Event];
    public static int GetId(this IEvent Event) => RegisteredEvents.IndexOf(Event);
    public static bool IsActive(this IEvent Event) => EventManager.CurrentEvent == Event;

    /// <summary>
    /// Returns a random event based on their rarity.
    /// </summary>
    /// <returns>a random event</returns>
    internal static IEvent GetRandomEvent() {
        var totalRarity = RegisteredEvents.Sum(Event => (int)Event.GetRarity());
        var random = UnityEngine.Random.Range(0, totalRarity);
        foreach (var @event in RegisteredEvents) {
            random -= (int)@event.GetRarity();
            if (random <= 0) return @event;
        }

        Plugin.Logger.LogFatal($"Failed to get random event! (totalRarity: {totalRarity}, random: {random})");
        return GetEvent<NoneEvent>();
    }

    /// <summary>
    /// Returns a random event without taking their rarity into account.
    /// </summary>
    /// <returns>a random event</returns>
    internal static IEvent GetRandomEventWithoutRarity() {
        if (RegisteredEvents.Where(Event => Event.GetRarity() != EventRarity.Disabled).Random(out var @event))
            return @event;
        Plugin.Logger.LogFatal("Failed to get random event! (without rarity)");
        return GetEvent<NoneEvent>();
    }
}