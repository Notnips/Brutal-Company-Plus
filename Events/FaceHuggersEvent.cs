using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
public class FaceHuggersEvent : IEvent {
    public string Name => "Bring a shovel!";
    public string Description => "They just want a hug...";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Uncommon;

    public void ExecuteServer(SelectableLevel Level) {
        EnemySpawnManager.DraftEnemySpawn<CentipedeAI>(new EnemySpawnManager.SpawnInfo(15, Immediate: false));
    }

    public void ExecuteClient(SelectableLevel Level) { }
}