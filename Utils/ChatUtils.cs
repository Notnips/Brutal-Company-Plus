// ReSharper disable MemberCanBePrivate.Global

namespace BrutalCompanyPlus.Utils;

public static class ChatUtils {
    public static void Send(string Message, bool Clear = false) {
        if (Clear) ChatUtils.Clear();
        HUDManager.Instance.AddTextToChatOnServer(Message);
    }

    public static void SendLocal(string Message, bool Clear = false) {
        if (Clear) ChatUtils.Clear();
        HUDManager.Instance.AddChatMessage(Message);
    }

    private static void Clear() => Send(
        "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n"
    );

    internal static void NotifyError() =>
        SendLocal($"An error occurred in {PluginInfo.PLUGIN_NAME}. Check the console for more information.");
}