// ReSharper disable InconsistentNaming

using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using HarmonyLib;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
[HarmonyPatch]
public class BlobEvolutionEvent : IEvent {
    // speed at which a blob travels during their normal behaviour
    private const float NormalSpeed = 3.8f;

    // how fast blobs open doors, until 1 second is reached. 1/OpenDoorSpeedMultiplier = seconds until open
    private const float OpenDoorSpeedMultiplier = 1.5f; // 0.6 seconds

    public string Name => "They are EVOLVING?!";
    public string Description => "Here I was, thinking that Pokémon was the only game with evolutions.";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Uncommon;

    public void ExecuteServer(SelectableLevel Level) {
        EnemySpawnManager.DraftEnemySpawn<BlobAI>(new EnemySpawnManager.SpawnInfo(3));
        EnemySpawnManager.DraftEnemySpawn<BlobAI>(new EnemySpawnManager.SpawnInfo(1, Outside: true));
    }

    public void ExecuteClient(SelectableLevel Level) { }

    [HarmonyPostfix, HarmonyPatch(typeof(BlobAI), "Update")]
    private static void EnemyAIPatch(ref BlobAI __instance) {
        if (!__instance.IsOwner || !EventManager.IsActive<BlobEvolutionEvent>()) return;
        __instance.agent.speed = NormalSpeed;
        __instance.openDoorSpeedMultiplier = OpenDoorSpeedMultiplier;
    }
}