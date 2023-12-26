// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

using System.Collections.Generic;

namespace BrutalCompanyPlus.Utils;

internal static class LevelDefaults {
    // @formatter:off
    internal static readonly Dictionary<string, Dictionary<string, int>> DefaultEnemyRarityValues = new() {
        { LevelNames.Experimentation, new() {
            { "Centipede", 50 }, { "Bunker Spider", 75 }, { "Hoarding bug", 80 }, { "Flowerman", 30 }, { "Crawler", 15 }, { "Blob", 25 }, { "Girl", 2 },
            { "Puffer", 10 }, { "Nutcracker", 15 }, { "Spring", 5 }, { "Jester", 1 }, { "Masked", 1 }, { "Lasso", 1 }
        } },
        { LevelNames.Assurance, new() {
            { "Centipede", 50 }, { "Bunker Spider", 40 }, { "Hoarding bug", 50 }, { "Flowerman", 30 }, { "Crawler", 15 }, { "Blob", 25 }, { "Girl", 2 },
            { "Puffer", 40 }, { "Nutcracker", 15 }, { "Spring", 25 }, { "Jester", 3 }, { "Masked", 3 }, { "Lasso", 1 }
        } },
        { LevelNames.Vow, new() {
            { "Centipede", 50 }, { "Bunker Spider", 40 }, { "Hoarding bug", 50 }, { "Flowerman", 30 }, { "Crawler", 20 }, { "Blob", 25 }, { "Girl", 5 },
            { "Puffer", 40 }, { "Nutcracker", 20 }, { "Spring", 40 }, { "Jester", 15 }, { "Masked", 10 }, { "Lasso", 0 }
        } },
        { LevelNames.Offense, new() {
            { "Centipede", 50 }, { "Bunker Spider", 40 }, { "Hoarding bug", 50 }, { "Flowerman", 30 }, { "Crawler", 20 }, { "Blob", 25 }, { "Girl", 5 },
            { "Puffer", 40 }, { "Nutcracker", 20 }, { "Spring", 40 }, { "Jester", 15 }, { "Masked", 10 }, { "Lasso", 0 }
        } },
        { LevelNames.March, new() {
            { "Centipede", 50 }, { "Bunker Spider", 40 }, { "Hoarding bug", 50 }, { "Flowerman", 30 }, { "Crawler", 20 }, { "Blob", 25 }, { "Girl", 10 },
            { "Puffer", 40 }, { "Nutcracker", 20 }, { "Spring", 40 }, { "Jester", 15 }, { "Masked", 10 }, { "Lasso", 0 }
        } },
        { LevelNames.Rend, new() {
            { "Centipede", 35 }, { "Bunker Spider", 25 }, { "Hoarding bug", 30 }, { "Flowerman", 56 }, { "Crawler", 50 }, { "Blob", 40 }, { "Girl", 25 },
            { "Puffer", 40 }, { "Nutcracker", 30 }, { "Spring", 58 }, { "Jester", 40 }, { "Masked", 10 }, { "Lasso", 2 }
        } },
        { LevelNames.Dine, new() {
            { "Centipede", 35 }, { "Bunker Spider", 25 }, { "Hoarding bug", 30 }, { "Flowerman", 56 }, { "Crawler", 50 }, { "Blob", 40 }, { "Girl", 25 },
            { "Puffer", 40 }, { "Nutcracker", 30 }, { "Spring", 58 }, { "Jester", 40 }, { "Masked", 10 }, { "Lasso", 2 }
        } },
        { LevelNames.Titan, new() {
            { "Centipede", 35 }, { "Bunker Spider", 25 }, { "Hoarding bug", 30 }, { "Flowerman", 56 }, { "Crawler", 60 }, { "Blob", 40 }, { "Girl", 25 },
            { "Puffer", 40 }, { "Nutcracker", 30 }, { "Spring", 58 }, { "Jester", 40 }, { "Masked", 10 }, { "Lasso", 2 }
        } },
        { LevelNames.Custom, new() {
            { "Centipede", 35 }, { "Bunker Spider", 25 }, { "Hoarding bug", 30 }, { "Flowerman", 56 }, { "Crawler", 60 }, { "Blob", 40 }, { "Girl", 25 },
            { "Puffer", 40 }, { "Nutcracker", 30 }, { "Spring", 58 }, { "Jester", 40 }, { "Masked", 10 }, { "Lasso", 2 }
        } }
    };
    // @formatter:on
}