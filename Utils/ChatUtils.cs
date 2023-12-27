namespace BrutalCompanyPlus.Utils;

public static class ChatUtils {
    public static void Send(string Message, bool Clear = false) {
        if (Clear) ChatUtils.Clear();
        HUDManager.Instance.AddTextToChatOnServer(Message);
    }

    private static void Clear() => Send(
        "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n"
    );
}