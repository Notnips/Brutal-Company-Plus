using System.Collections.Generic;
using BepInEx;
using BrutalCompanyPlus.Api;

namespace BrutalCompanyPlus.Config;

#pragma warning disable CS0618 // Type or member is obsolete
internal static class EventConfig {
    private const string Category = "Events";

    internal static void RegisterWith(BaseUnityPlugin FromPlugin, ref List<IEvent> Events) {
        // Apply user configuration
        foreach (var @event in Events) {
            @event.Rarity = FromPlugin.Config.Bind(
                Category, @event.Name, @event.Rarity, @event.Description
            ).Value;
        }

        // Remove disabled events
        Events.RemoveAll(Event => Event.Rarity == EventRarity.Disabled);
    }
}