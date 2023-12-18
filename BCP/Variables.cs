using System;
using System.Collections.Generic;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

namespace BrutalCompanyPlus.BCP
{
    public static class Variables
    {

        // Clean this up eventually
        public static bool BlobsHaveEvolved = false;
        public static bool InstaJester = false;

        public static bool WaitUntilPlayerInside = false;

        // Tribute Variables
        public static PlayerControllerB SacrificeTarget = null;
        public static bool Tribute = false;
        public static float SacrificeTargetChangeTimer = 0f;
        public static readonly float TargetChangeInterval = 30f;

        // The Beast Inside Variables
        public static readonly HashSet<int> DogsSpawnedInside = new();
        public static bool DogForceOwnership = false;
        public static bool SpawnDogsInside = false;
        public static bool DogIsLunging = false;

        // Inside Out Variables
        public static readonly HashSet<int> SpawnedInsideOutID = new();
        public static bool InsideOutOwnership = false;
        public static bool SpawnInsideOut = false;

        public static readonly List<SpawnableEnemyWithRarity> OriginalEnemyListWithRarity = new();
        
        public static bool TheRumbling = false;

        public static bool Landed = false;
        public static bool TakeOffExecuted = false;

        public static int SurpriseLandmines;

        public static GameObject Landmine;

        public static GameObject Turret;

        public static readonly List<GameObject> ObjectsToCleanUp = new();

        public static float SlSpawnTimer;

        public static bool ShouldSpawnTurret;

        public static bool TurretSpawned = false;

        public static EventEnum LastEvent = EventEnum.None;

        public static float CurrentLevelHeatValue = 0f;

        public static SelectableLevel CurrentLevel = null;

        public static readonly List<SelectableLevel> LevelsModified = new();

        public static Dictionary<SpawnableEnemyWithRarity, int> EnemyRarities;

        public static Dictionary<SpawnableEnemyWithRarity, AnimationCurve> EnemyPropCurves;

        public static readonly Dictionary<Type, bool> AIPresence = new() {
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
            //{ typeof(LassoManAI), false },
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
        public static readonly Dictionary<string, Dictionary<string, int>> DefaultEnemyRarities = new();

        //Lightning Variables
        public static readonly Random SmiteSeed = new();
        public static GameObject[] SmiteOutsideNodes;
        public static Vector3 LastRandomStrikePosition;
        public static float TimeAtLastStrike;
        public static float RandomThunderTime;
        public static NavMeshHit SmiteNavHit;

        public static float LightningStrikeInterval = 3.0f; // Default interval in seconds
        public static float LastStrikeTime;
        public static bool SmiteEnabled = false;

        //Enemy Spawn Variables
        public static readonly List<EnemySpawnInfo> PresetEnemiesToSpawn = new();
        public struct EnemySpawnInfo
        {
            public readonly Type EnemyType;
            public readonly int Amount;
            public readonly SpawnLocation Location;
            public readonly bool ForceInside;
            public readonly bool ForceOutside;

            public EnemySpawnInfo(Type EnemyType, int Amount, SpawnLocation Location, bool Forceinside, bool Forceoutside)
            {
                this.EnemyType = EnemyType;
                this.Amount = Amount;
                this.Location = Location;
                ForceInside = Forceinside;
                ForceOutside = Forceoutside;
            }
        }
    }
}
