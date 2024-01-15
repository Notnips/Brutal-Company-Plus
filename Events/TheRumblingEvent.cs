// ReSharper disable InconsistentNaming

using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using BrutalCompanyPlus.Utils;
using HarmonyLib;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
[HarmonyPatch]
public class TheRumblingEvent : IEvent {
    // speed at which a forest giant travels during their normal behaviour
    private const float NormalSpeed = 6f;

    public string Name => "The Rumbling";
    public string Description => "Fix the Barricades!";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Uncommon;

    public void ExecuteServer(SelectableLevel Level) {
        EnemySpawnManager.DraftEnemySpawn<ForestGiantAI>(new EnemySpawnManager.SpawnInfo(Level.name switch {
            LevelNames.Experimentation => 6,
            LevelNames.Assurance => 8,
            LevelNames.Vow => 8,
            LevelNames.Offense => 8,
            LevelNames.March => 8,
            LevelNames.Rend => 10,
            LevelNames.Dine => 10,
            LevelNames.Titan => 8,
            _ => 8 // custom level
        }, Outside: true));
    }

    public void ExecuteClient(SelectableLevel Level) { }

    [HarmonyPostfix, HarmonyPatch(typeof(ForestGiantAI), "Update")]
    private static void EnemyAIPatch(ref ForestGiantAI __instance) {
        if (!__instance.IsOwner || !EventManager.IsActive<TheRumblingEvent>()) return;
        if (__instance.currentBehaviourStateIndex != 1 || __instance.inEatingPlayerAnimation) return;
        __instance.agent.speed = NormalSpeed;
    }
}