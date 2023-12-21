using System.Collections.Generic;

namespace BrutalCompanyPlus.Utils;

internal static class Diagnostics {
    private static readonly List<string> Errors = new();

    internal static void AddError(string Error) {
        Errors.Add(Error);
        Plugin.Logger.LogError(Error);
    }

    internal static bool HasErrors => !Errors.IsEmpty();

    internal static string CollectErrors() {
        var errors = string.Join("\n", Errors);
        Errors.Clear();
        return errors;
    }
}