// ReSharper disable InconsistentNaming

using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
[HarmonyPatch]
public class InstaJesterEvent : IEvent {
    private const float MinTimer = 1f;
    private const float MaxTimer = 5f;
    
    public string Name => "Pop goes the.. HOLY FUC-";
    public string Description => "Lord have mercy on your soul.";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Rare;

    public void ExecuteServer(SelectableLevel Level) {
        EnemySpawnManager.DraftEnemySpawn<JesterAI>(new EnemySpawnManager.SpawnInfo(2));
    }

    public void ExecuteClient(SelectableLevel Level) { }

    [HarmonyPostfix, HarmonyPatch(typeof(JesterAI), "SetJesterInitialValues")]
    private static void EnemyAIPatch(ref JesterAI __instance) {
        if (!EventManager.IsActive<InstaJesterEvent>()) return;
        __instance.popUpTimer = Random.Range(MinTimer, MaxTimer);
    }
}