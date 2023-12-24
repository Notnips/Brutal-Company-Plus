using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
public class HoarderTownEvent : IEvent {
    public string Name => "Hoarder Town";
    public string Description => "Come on, those are mine!";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Rare;

    public void ExecuteServer(SelectableLevel Level) {
        EnemySpawnManager.DraftEnemySpawn<HoarderBugAI>(new EnemySpawnManager.SpawnInfo(8));
        EnemySpawnManager.DraftEnemySpawn<HoarderBugAI>(new EnemySpawnManager.SpawnInfo(8, Outside: true));
    }

    public void ExecuteClient(SelectableLevel Level) { }
}