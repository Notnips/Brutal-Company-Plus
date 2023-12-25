// ReSharper disable InconsistentNaming

using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using BrutalCompanyPlus.Utils;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
[HarmonyPatch]
public class SpringbreakEvent : IEvent {
    public string Name => "Springbreak";
    public string Description => "Don't get a concussion!";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Rare;

    public void ExecuteServer(SelectableLevel Level) {
        EnemySpawnManager.DraftEnemySpawn<SpringManAI>(new EnemySpawnManager.SpawnInfo(3, Outside: true));
    }

    public void ExecuteClient(SelectableLevel Level) { }

    [HarmonyPrefix, HarmonyPatch(typeof(EnemyAI), "StartSearch")]
    private static void EnemyAIPatch(ref EnemyAI __instance, ref Vector3 startOfSearch) {
        if (!__instance.IsOwner || !EventManager.IsActive<SpringbreakEvent>()) return;
        if (__instance is not SpringManAI || !__instance.isOutside) return;
        // Modify starting position to start searching from the ship position
        var shipLocation = StartOfRound.Instance.playerSpawnPositions[0].position;
        startOfSearch = BcpUtils.GetNearbyLocation(shipLocation);
    }
}