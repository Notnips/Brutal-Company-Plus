using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
public class TheyAreShyEvent : IEvent {
    public string Name => "Don't look... away?";
    public string Description => "Instructions unclear.";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Rare;

    public void ExecuteServer(SelectableLevel Level) {
        EnemySpawnManager.DraftEnemySpawn<SpringManAI>(new EnemySpawnManager.SpawnInfo(3));
        EnemySpawnManager.DraftEnemySpawn<FlowermanAI>(new EnemySpawnManager.SpawnInfo(3));
        EnemySpawnManager.DraftEnemySpawn<SpringManAI>(new EnemySpawnManager.SpawnInfo(2, Outside: true));
        EnemySpawnManager.DraftEnemySpawn<FlowermanAI>(new EnemySpawnManager.SpawnInfo(3, Outside: true));
    }

    public void ExecuteClient(SelectableLevel Level) { }
}