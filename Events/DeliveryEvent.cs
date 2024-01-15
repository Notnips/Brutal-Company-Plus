using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Utils;
using JetBrains.Annotations;

namespace BrutalCompanyPlus.Events;

[UsedImplicitly]
public class DeliveryEvent : IEvent {
    private const int MinItems = 2;
    private const int MaxItems = 9;

    public string Name => "ICE SCREAM!!!";
    public string Description => "Mommy, the ice cream truck is here!";
    public EventPositivity Positivity => EventPositivity.Positive;
    public EventRarity DefaultRarity => EventRarity.Rare;

    public void ExecuteServer(SelectableLevel Level) {
        var items = Singleton.Terminal.buyableItemsList.Length;
        var amount = UnityEngine.Random.Range(MinItems, MaxItems);
        for (var i = 0; i < amount; i++) {
            Singleton.Terminal.orderedItemsFromTerminal.Add(UnityEngine.Random.Range(0, items));
        }
    }

    public void ExecuteClient(SelectableLevel Level) { }
}