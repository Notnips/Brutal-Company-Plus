using System;
using static BrutalCompanyPlus.Objects.MoonHeatManager;

namespace BrutalCompanyPlus.Utils;

internal static class ParseHelpers {
    private static readonly string RangeError =
        $"must be between {MoonHeatRange.Min} and {MoonHeatRange.Max}";

    internal static (int, int, LevelWeatherType) ParseHeatCurvePoint(string Point) {
        var values = Point.Split(':');

        if (values.Length != 3)
            throw new ParseException("invalid format");
        if (!int.TryParse(values[0], out var start))
            throw new ParseException("invalid start value");
        if (!MoonHeatRange.IntInRange(start))
            throw new ParseException($"start value out of range, {RangeError}");
        if (!int.TryParse(values[1], out var end))
            throw new ParseException("invalid end value");
        if (!MoonHeatRange.IntInRange(Math.Max(0, end - 1)))
            throw new ParseException($"end value out of range, {RangeError}");
        if (!Enum.TryParse(values[2], out LevelWeatherType type))
            throw new ParseException("invalid weather type");

        return (start, end, type);
    }

    internal static (int, int, int, int) ParseScrapValues(string Entries) {
        var values = Entries.Split(',');

        if (values.Length != 4)
            throw new ParseException("invalid format");
        if (!int.TryParse(values[0], out var minScrap))
            throw new ParseException("invalid minScrap value");
        if (!int.TryParse(values[1], out var maxScrap))
            throw new ParseException("invalid maxScrap value");
        if (!int.TryParse(values[2], out var minTotalScrapValue))
            throw new ParseException("invalid minTotalScrapValue value");
        if (!int.TryParse(values[3], out var maxTotalScrapValue))
            throw new ParseException("invalid maxTotalScrapValue value");

        return (minScrap, maxScrap, minTotalScrapValue, maxTotalScrapValue);
    }

    internal static (string, int) ParseEnemyRarityEntry(string Entry) {
        var values = Entry.Split(':');
        string enemy; // initialized below

        if (values.Length != 2)
            throw new ParseException("invalid format");
        if (string.IsNullOrEmpty(enemy = values[0]))
            throw new ParseException("invalid enemy name");
        if (!int.TryParse(values[1], out var rarity))
            throw new ParseException("invalid rarity value");

        return (enemy, rarity);
    }
}

internal class ParseException : Exception {
    public ParseException(string Message) : base(Message) { }
    public override string ToString() => Message;
}