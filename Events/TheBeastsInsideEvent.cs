// ReSharper disable InconsistentNaming

using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using BrutalCompanyPlus.Utils;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
public class TheBeastsInsideEvent : IEvent {
    public string Name => "The Beasts Inside";
    public string Description => "Ssshhhh...";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.Uncommon;

    public void ExecuteServer(SelectableLevel Level) {
        var amount = Level.name switch {
            LevelNames.Experimentation => 2,
            LevelNames.Assurance => 2,
            LevelNames.Vow => 2,
            LevelNames.Offense => 3,
            LevelNames.March => 3,
            LevelNames.Rend => 4,
            LevelNames.Dine => 4,
            LevelNames.Titan => 6,
            _ => 4 // custom level
        };
        EnemySpawnManager.DraftEnemySpawn<MouthDogAI>(new EnemySpawnManager.SpawnInfo(amount));
    }

    public void ExecuteClient(SelectableLevel Level) { }
}