using System.Collections.Generic;
using BepInEx;
using BrutalCompanyPlus.Api;

namespace BrutalCompanyPlus.Config;

#pragma warning disable CS0618 // Type or member is obsolete
internal static class EventConfig {
    private const string Category = "Event Rarities";

    internal static void RegisterWith(BaseUnityPlugin FromPlugin, ref List<IEvent> Events) {
        // Apply user configuration
        foreach (var @event in Events) {
            // Apply event rarity
            EventRegistry.EventRarityValues[@event] = FromPlugin.Config.Bind(
                Category, @event.Name, @event.DefaultRarity, @event.Description
            ).Value;
        }

        // Remove disabled events
        Events.RemoveAll(Event => Event.GetRarity() == EventRarity.Disabled);
    }
}