using System;

namespace BrutalCompanyPlus.Api;

public interface IEvent {
    /// <summary>
    /// Name of the event.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Description of the event.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Whether this event (and its effects) is positive, negative or neutral.
    /// See <see cref="EventPositivity"/> for all possible values.
    /// </summary>
    public EventPositivity Positivity { get; }

    /// <summary>
    /// The rarity of this event. The higher the rarity, the less likely it is to occur.
    /// See <see cref="EventRarity"/> for all possible values.
    /// <para><b>Note: May be overridden by user configuration.</b></para>
    /// </summary>
    public EventRarity Rarity { get; internal set; }

    /// <summary>
    /// Execute the event on the server.
    /// </summary>
    /// <param name="Level">the current level</param>
    public void ExecuteServer(SelectableLevel Level);

    /// <summary>
    /// Execute the event on the client.
    /// </summary>
    /// <param name="Level">the current level</param>
    public void ExecuteClient(SelectableLevel Level);

    /// <summary>
    /// Called when the event is over.
    /// </summary>
    public void OnEnd();
}

public enum EventPositivity {
    /// <summary>
    /// Players will benefit from this event.
    /// </summary>
    Positive,

    /// <summary>
    /// Players will neither benefit nor be harmed by this event.
    /// </summary>
    Neutral,

    /// <summary>
    /// Players will be harmed by this event.
    /// </summary>
    Negative,

    /// <summary>
    /// Players will DEFINITELY benefit from this event and should be rare.
    /// </summary>
    Golden
}

public enum EventRarity {
    /// <summary>
    /// For configuration purposes only. <b>Do not use.</b>
    /// </summary>
    [Obsolete("For configuration purposes only. Do not use.")]
    Disabled = -1,

    /// <summary>
    /// This event will occur very often.
    /// </summary>
    Common = 100,

    /// <summary>
    /// This event will occur often.
    /// </summary>
    Uncommon = 85,

    /// <summary>
    /// This event will occur rarely.
    /// </summary>
    Rare = 60,

    /// <summary>
    /// This event will occur very rarely.
    /// </summary>
    VeryRare = 40,

    /// <summary>
    /// This event will occur extremely rarely.
    /// </summary>
    UltraRare = 20
}