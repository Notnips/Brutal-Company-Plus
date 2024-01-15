using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Objects;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
public class ResetHeatEvent : IEvent {
    public string Name => "All moon heat has been reset!";
    public string Description => "Seems like Pluto just passed by.";
    public EventPositivity Positivity => EventPositivity.Positive;
    public EventRarity DefaultRarity => EventRarity.Uncommon;

    public void ExecuteServer(SelectableLevel Level) {
        MoonHeatManager.ResetHeatValues();
    }

    public void ExecuteClient(SelectableLevel Level) { }
}