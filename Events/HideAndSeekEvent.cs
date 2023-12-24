using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
public class HideAndSeekEvent : IEvent {
    public string Name => "Hide and Seek";
    public string Description => "She's gonna find you...";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Uncommon;

    public void ExecuteServer(SelectableLevel Level) {
        EnemySpawnManager.DraftEnemySpawn<DressGirlAI>(new EnemySpawnManager.SpawnInfo(5));
    }

    public void ExecuteClient(SelectableLevel Level) { }
}