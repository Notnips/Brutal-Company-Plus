using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
public class ShadowRealmEvent : IEvent {
    public string Name => "The shadows are roaming";
    public string Description => "Did you hear that?";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Rare;

    public void ExecuteServer(SelectableLevel Level) {
        EnemySpawnManager.DraftEnemySpawn<FlowermanAI>(new EnemySpawnManager.SpawnInfo(6, Outside: true));
    }

    public void ExecuteClient(SelectableLevel Level) { }
}