using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BrutalCompanyPlus.Api;
using BrutalCompanyPlus.Config;
using BrutalCompanyPlus.Objects;
using BrutalCompanyPlus.Utils;
using HarmonyLib;
using UnityEngine;

namespace BrutalCompanyPlus;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInProcess("Lethal Company.exe")]
public class Plugin : BaseUnityPlugin {
    internal new static ManualLogSource Logger;
    internal static GameObject BCNetworkManagerPrefab;

    private void Awake() {
        Logger = base.Logger;
        PluginConfig.Bind(this);
        EventRegistry.AutoRegister(this);
        BcpUtils.InitializeNetcode();
        InitializeBCNetworkManager();
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        Logger.LogWarning($"{PluginInfo.PLUGIN_NAME} initialized...");
    }

    private static void InitializeBCNetworkManager() {
        Logger.LogWarning($"Initializing {nameof(BCNetworkManager)}...");
        var bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly()
            .GetManifestResourceStream($"{PluginInfo.PLUGIN_NAME}.Assets.brutalcompanyplus"));
        BCNetworkManagerPrefab = bundle.LoadAsset<GameObject>("Assets/BCNetworkManager.prefab");
        BCNetworkManagerPrefab.AddComponent<BCNetworkManager>();
        bundle.Unload(false);
    }
}