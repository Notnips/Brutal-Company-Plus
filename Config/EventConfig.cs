using System;
using System.Collections.Generic;
using BepInEx;
using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Utils;

namespace BrutalCompanyPlus.Config;

#pragma warning disable CS0618 // Type or member is obsolete
internal static class EventConfig {
    private const string Category = "Event Rarities";

    internal static void RegisterWith(BaseUnityPlugin FromPlugin, ref List<IEvent> Events) {
        // Apply user configuration
        foreach (var @event in Events) {
            try {
                // Sanitize event name (illegal characters in config keys)
                var eventName = @event.Name.Replace("'", "");
                // Apply event rarity
                EventRegistry.EventRarityValues[@event] = FromPlugin.Config.Bind(
                    Category, eventName, @event.DefaultRarity, @event.Description
                ).Value;
            } catch (Exception e) {
                // Log the error
                Plugin.Logger.LogError(e.ToString());
                var from = FromPlugin.Info.Metadata.GUID != PluginInfo.PLUGIN_GUID
                    ? $" from {FromPlugin.Info.Metadata.Name}"
                    : "";
                Diagnostics.AddError(
                    $"Failed to register event \"{@event.Name}\"{from}! " +
                    $"See console for details.");
                // Apply default rarity as a fallback and continue
                EventRegistry.EventRarityValues[@event] = @event.DefaultRarity;
            }
        }
    }
}