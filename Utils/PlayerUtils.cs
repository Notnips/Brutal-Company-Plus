// ReSharper disable MemberCanBePrivate.Global

using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;

namespace BrutalCompanyPlus.Utils;

public static class PlayerUtils {
    /// <summary>
    /// Returns all players that are currently connected to the server.
    /// </summary>
    public static IEnumerable<PlayerControllerB> AllPlayers =>
        StartOfRound.Instance.allPlayerScripts.Where(Player => Player.isPlayerControlled);

    /// <summary>
    /// Returns all players that are currently alive.
    /// </summary>
    public static IEnumerable<PlayerControllerB> AlivePlayers =>
        AllPlayers.Where(Player => !Player.isPlayerDead);

    /// <summary>
    /// Returns all players that are currently outside of the ship and factory.
    /// </summary>
    public static IEnumerable<PlayerControllerB> OutsidePlayers =>
        AlivePlayers.Where(Player => !Player.isInHangarShipRoom && !Player.isInsideFactory);
}