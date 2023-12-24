using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using JetBrains.Annotations;
using UnityEngine;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
public class UnfairCompanyEvent : IEvent {
    public string Name => "Unfair Company";
    public string Description => "Sometimes, life just ain't fair.";
    public EventPositivity Positivity => EventPositivity.Negative;
    public EventRarity DefaultRarity => EventRarity.VeryRare;

    public void ExecuteServer(SelectableLevel Level) {
        // ReSharper disable once VariableHidesOuterVariable
        LevelManager.ModifyLevelProperties(Level, Level => {
                Level.outsideEnemySpawnChanceThroughDay = new AnimationCurve(
                    new Keyframe(0f, 999f),
                    new Keyframe(21f, 999f)
                );
                Level.enemySpawnChanceThroughoutDay = new AnimationCurve(
                    new Keyframe(0f, 500f),
                    new Keyframe(0.5f, 500f)
                );
            },
            nameof(SelectableLevel.outsideEnemySpawnChanceThroughDay),
            nameof(SelectableLevel.enemySpawnChanceThroughoutDay)
        );
    }

    public void ExecuteClient(SelectableLevel Level) { }
}