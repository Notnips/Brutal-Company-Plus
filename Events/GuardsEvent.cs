using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
public class GuardsEvent : IEvent {
    public string Name => "They guard this place!";
    public string Description => "Better not get kicked out again...";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Uncommon;

    public void ExecuteServer(SelectableLevel Level) {
        EnemySpawnManager.DraftEnemySpawn<NutcrackerEnemyAI>(new EnemySpawnManager.SpawnInfo(4, Outside: true));
    }

    public void ExecuteClient(SelectableLevel Level) { }
}