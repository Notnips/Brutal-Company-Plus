using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyPlus.Objects;

public class BCNetworkManager : NetworkBehaviour {
    private const string Tag = $"[{nameof(BCNetworkManager)}]";

    public static BCNetworkManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
        Log($"{nameof(BCNetworkManager)} initialized!");
    }

    private static void Log(string Message) => Plugin.Logger.LogWarning($"{Tag} {Message}");
}