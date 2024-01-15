// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using BrutalCompanyPlus.Utils;
using GameNetcodeStuff;
using HarmonyLib;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
[HarmonyPatch]
public class OutsideTheBoxEvent : IEvent {
    public string Name => "Outside the box!";
    public string Description => "Creepy...";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Rare;

    public void ExecuteServer(SelectableLevel Level) {
        EnemySpawnManager.DraftEnemySpawn<JesterAI>(new EnemySpawnManager.SpawnInfo(2, Outside: true));
    }

    public void ExecuteClient(SelectableLevel Level) { }

    private static bool HookResetFlag(PlayerControllerB player, EnemyAI Instance) {
        // If the event is active and the enemy is outside;
        if (EventManager.IsActive<OutsideTheBoxEvent>() && Instance.isOutside) {
            // check if the player is not inside the factory.
            return !player.isInsideFactory;
        }

        // Otherwise, just return the original value.
        return player.isInsideFactory;
    }

    [HarmonyTranspiler, HarmonyPatch(typeof(JesterAI), "Update")]
    private static IEnumerable<CodeInstruction> EnemyAIPatch(IEnumerable<CodeInstruction> instructions) {
        var insideField = AccessTools.Field(typeof(PlayerControllerB), nameof(PlayerControllerB.isInsideFactory));
        var hookMethod = AccessTools.Method(typeof(OutsideTheBoxEvent), nameof(HookResetFlag));

        var ok = false;
        var il = instructions.ToList();
        for (var i = 1; i < il.Count; i++) { // Skip the first instruction to account for i - 1
            // Find the first instance of "if (... && StartOfRound.Instance.allPlayerScripts[i].isInsideFactory)"
            if (!il[i - 1].LoadsField(insideField) || il[i].opcode != OpCodes.Brfalse) continue;
            // Insert a new instruction to load the instance of this class onto the stack
            il[i - 1] = new CodeInstruction(OpCodes.Ldarg_0);
            // Insert a new instruction to call our hook method
            il.Insert(i, new CodeInstruction(OpCodes.Call, hookMethod));
            // This makes "if (... && HookResetFlag(StartOfRound.Instance.allPlayerScripts[i], this))"
            // We only need to do this once, so we're done here.
            ok = true;
            break;
        }

        if (!ok)
            Diagnostics.AddError(
                $"Failed to patch JesterAI.Update()! (" +
                $"event: {nameof(OutsideTheBoxEvent)}, " +
                $"ver: {PluginInfo.PLUGIN_VERSION})");
        return il.AsEnumerable();
    }
}