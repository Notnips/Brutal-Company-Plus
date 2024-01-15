using BrutalCompanyPlus.Api;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
public class NoneEvent : IEvent {
    public string Name => "Nothing happened today!";
    public string Description => "Just another Tuesday.";
    public EventPositivity Positivity => EventPositivity.Positive;
    public EventRarity DefaultRarity => EventRarity.Uncommon;

    public void ExecuteServer(SelectableLevel Level) { }
    public void ExecuteClient(SelectableLevel Level) { }
}