namespace BrutalCompanyPlus.Utils;

public static class ChatUtils {
    public static void Send(string Message, bool Clear = false) {
        HUDManager.Instance.AddTextToChatOnServer(Message);
        if (Clear) ChatUtils.Clear();
    }

    public static void Clear() => Send(
        "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n"
    );
}