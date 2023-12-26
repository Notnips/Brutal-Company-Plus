using System;
using JetBrains.Annotations;

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
    /// The default rarity of this event. The higher the rarity, the less likely it is to occur.
    /// See <see cref="EventRarity"/> for all possible values.
    /// <para><b>Note: May be overridden by user configuration.</b></para>
    /// </summary>
    public EventRarity DefaultRarity { get; }

    /// <summary>
    /// Whether this event can run on the current level.
    /// <para><b>You are not allowed to modify the level at this point,
    /// because it will not be reverted if the event is not chosen.</b></para>
    /// </summary>
    /// <param name="Level">the current level</param>
    /// <returns></returns>
    public bool CanRun([UsedImplicitly] SelectableLevel Level) => true;

    /// <summary>
    /// Execute the event on the host (IsHost == true).
    /// </summary>
    /// <param name="Level">the current level</param>
    public void ExecuteServer([UsedImplicitly] SelectableLevel Level);

    /// <summary>
    /// Execute the event on the client (IsHost == false).
    /// </summary>
    /// <param name="Level">the current level</param>
    public void ExecuteClient([UsedImplicitly] SelectableLevel Level);

    /// <summary>
    /// Runs every tick on the server while the event is active.
    /// </summary>
    public void UpdateServer() { }

    /// <summary>
    /// Called when the event is over, on both the server and the client.
    /// You may differenciate between the two by checking:
    /// <code>if (RoundManager.Instance.IsHost) { ... }</code>
    /// </summary>
    /// <param name="Level">the current level</param>
    public void OnEnd([UsedImplicitly] SelectableLevel Level) { }
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
    Disabled = 0,

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

public static class EventExtensions {
    /// <summary>
    /// Logs a message to the console on the INFO level with the event name prefixed.
    /// </summary>
    /// <param name="Event">the event</param>
    /// <param name="Message">the log message</param>
    public static void Log(this IEvent Event, string Message) => Plugin.Logger.LogInfo($"[Event ({Event.Name})] {Message}");
}