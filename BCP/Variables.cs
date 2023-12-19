using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GameNetcodeStuff;
using System.Linq;

namespace BrutalCompanyPlus.BCP
{
    public static class Variables
    {

        //Clean this up eventually

        public static bool BlobsHaveEvolved = false;
        public static bool InstaJester = false;

        public static bool WaitUntilPlayerInside = false;

        //Tribute Variables
        public static PlayerControllerB SacrificeTarget = null;
        public static bool Tribute = false;
        public static float sacrificeTargetChangeTimer = 0f;
        public static float TargetChangeInterval = 30f;

        //The Beast Inside Variables
        public static HashSet<int> DogsSpawnedInside = new HashSet<int>();
        public static bool DogForceOwnership = false;
        public static bool SpawnDogsInside = false;
        public static bool DogIsLunging = false;

        //Inside Out Variables
        public static HashSet<int> SpawnedInsideOutID = new HashSet<int>();
        public static bool InsideOutOwnership = false;
        public static bool SpawnInsideOut = false;


        public static List<SpawnableEnemyWithRarity> OriginalEnemyListWithRarity = new List<SpawnableEnemyWithRarity>();


        public static bool TheRumbling = false;

        public static float timeNearDoor = 0f;

        public static float requiredTime = 2f;

        public static bool Landed = false;
        public static bool TakeOffExecuted = false;

        public static TimeOfDay TOD;

        public static float messageTimer = 58f;

        public static int surpriseLandmines;

        public static GameObject landmine;

        public static GameObject turret;

        public static List<GameObject> objectsToCleanUp = new List<GameObject>();

        public static float slSpawnTimer;

        public static bool shouldSpawnTurret;

        public static bool TurretSpawned = false;

        public static bool loaded;

        public static EventEnum lastEvent = EventEnum.None;

        public static float CurrentLevelHeatValue = 0f;

        public static readonly Harmony _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

        public static SelectableLevel CurrentLevel = null;

        public static List<SelectableLevel> levelsModified = new List<SelectableLevel>();

        public static Dictionary<SelectableLevel, float> levelHeatVal;

        public static Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>> levelEnemySpawns;

        public static Dictionary<SpawnableEnemyWithRarity, int> enemyRaritys;

        public static Dictionary<SpawnableEnemyWithRarity, AnimationCurve> enemyPropCurves;

        public static ManualLogSource mls;

        public static Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>> originalEnemyLists = new Dictionary<SelectableLevel, List<SpawnableEnemyWithRarity>>();

        public static Dictionary<Type, bool> defaultRemovableEnemyTypes = new Dictionary<Type, bool>
        {
            { typeof(BlobAI), false },
            { typeof(CentipedeAI), false },
            { typeof(CrawlerAI), false },
            { typeof(DressGirlAI), false },
            { typeof(FlowermanAI), false },
            { typeof(HoarderBugAI), false },
            { typeof(JesterAI), false },
            { typeof(PufferAI), false },
            { typeof(SandSpiderAI), false },
            { typeof(SpringManAI), false },
            { typeof(NutcrackerEnemyAI), false },
            { typeof(MaskedPlayerEnemy), false }
        };

        public static Dictionary<Type, bool> aiPresence = new Dictionary<Type, bool>
        {
            { typeof(BaboonBirdAI), false },
            { typeof(BlobAI), false },
            { typeof(CentipedeAI), false },
            { typeof(CrawlerAI), false },
            { typeof(DoublewingAI), false },
            { typeof(DressGirlAI), false },
            { typeof(FlowermanAI), false },
            { typeof(ForestGiantAI), false },
            { typeof(HoarderBugAI), false },
            { typeof(JesterAI), false },
            { typeof(MouthDogAI), false },
            { typeof(PufferAI), false },
            { typeof(RedLocustBees), false },
            { typeof(SandSpiderAI), false },
            { typeof(SandWormAI), false },
            { typeof(SpringManAI), false },
            { typeof(NutcrackerEnemyAI), false },
            { typeof(MaskedPlayerEnemy), false }
        };

        //Default Values for EnemyRarity
        public static Dictionary<string, Dictionary<string, int>> DefaultEnemyRarities = new Dictionary<string, Dictionary<string, int>>();

        //Lightning Variables
        public static System.Random Smite_seed = new System.Random();
        public static GameObject[] Smite_outsideNodes;
        public static Vector3 lastRandomStrikePosition;
        public static float timeAtLastStrike;
        public static float randomThunderTime;
        public static NavMeshHit Smite_navHit;

        public static float lightningStrikeInterval = 3.0f; // Default interval in seconds
        public static float lastStrikeTime;
        public static bool SmiteEnabled = false;

        //Enemy Spawn Variables
        public static List<EnemySpawnInfo> presetEnemiesToSpawn = new List<EnemySpawnInfo>();
        public struct EnemySpawnInfo
        {
            public Type EnemyType;
            public int Amount;
            public SpawnLocation Location;
            public bool ForceInside;
            public bool ForceOutside;

            public EnemySpawnInfo(Type enemyType, int amount, SpawnLocation location, bool forceinside, bool forceoutside)
            {
                EnemyType = enemyType;
                Amount = amount;
                Location = location;
                ForceInside = forceinside;
                ForceOutside = forceoutside;
            }
        }
    }
}
