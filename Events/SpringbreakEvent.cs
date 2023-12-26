using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
public class SpringbreakEvent : IEvent {
    public string Name => "Springbreak";
    public string Description => "Don't get a concussion!";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Rare;

    public void ExecuteServer(SelectableLevel Level) {
        EnemySpawnManager.DraftEnemySpawn<SpringManAI>(new EnemySpawnManager.SpawnInfo(3, Outside: true));
    }

    public void ExecuteClient(SelectableLevel Level) { }
}