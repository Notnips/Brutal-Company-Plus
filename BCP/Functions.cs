using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using static BrutalCompanyPlus.BCP.Variables;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace BrutalCompanyPlus.BCP
{
    public static class Functions
    {
        public static void AddSpecificEnemiesForEvent(SelectableLevel NewLevel, List<Type> EnemyAITypes)
        {
            var levels = StartOfRound.Instance.levels;
            foreach (var level in levels)
            {
                foreach (var spawnable in level.Enemies)
                {
                    var enemyPrefab = spawnable.enemyType.enemyPrefab;

                    foreach (var aiType in EnemyAITypes)
                    {
                        //Check if this enemy has the AI component we're looking for
                        if (enemyPrefab.GetComponent(aiType) != null)
                        {
                            //Check if this enemy is not already in the new level
                            if (!NewLevel.Enemies.Any(E => E.enemyType.enemyPrefab == enemyPrefab))
                            {
                                NewLevel.Enemies.Add(spawnable);
                                Mls.LogInfo($"Added specific Enemy: > {spawnable.enemyType.enemyPrefab.name} < for event");
                                //BcpLogger.Log($"Added specific Enemy: > {spawnable.enemyType.enemyPrefab.name} < for event");
                            }
                        }
                    }
                }
            }
        }

        //This is designed to check for Enemy Type existance and add it to the Enemy list if it doesn't exist
        public static GameObject FindEnemyPrefabByType(Type EnemyType, List<SpawnableEnemyWithRarity> EnemyList, SelectableLevel NewLevel)
        {
            foreach (var enemy in EnemyList)
            {
                if (enemy.enemyType.enemyPrefab.GetComponent(EnemyType) != null)
                {
                    return enemy.enemyType.enemyPrefab;
                }
            }

            //If the enemy type is not found, try to add it to the newLevel
            AddSpecificEnemiesForEvent(NewLevel, new List<Type> { EnemyType });

            //Search again in the newLevel's enemy list
            foreach (var enemy in NewLevel.Enemies)
            {
                if (enemy.enemyType.enemyPrefab.GetComponent(EnemyType) != null)
                {
                    return enemy.enemyType.enemyPrefab;
                }
            }

            throw new Exception($"Enemy type {EnemyType.Name} not found and could not be added.");
        }

        public static EnemyAI SpawnEnemyOutside(Type EnemyType, bool ForceOutside)
        {

            GameObject enemyPrefab = null;

            //Find enemy prefab by type
            if (ForceOutside)
            {
                enemyPrefab = FindEnemyPrefabByType(EnemyType, RoundManager.Instance.currentLevel.Enemies, CurrentLevel);
            }
            else
            {
                enemyPrefab = FindEnemyPrefabByType(EnemyType, RoundManager.Instance.currentLevel.OutsideEnemies, CurrentLevel);
            }
           

            var spawnPoints = GameObject.FindGameObjectsWithTag("OutsideAINode");
            var spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;

            //Instantiate enemy
            var enemyObject = Object.Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemyObject.GetComponentInChildren<NetworkObject>().Spawn(true);

            var enemyAI = enemyObject.GetComponent<EnemyAI>();

            if (ForceOutside)
            {
                enemyAI.enemyType.isOutsideEnemy = true;
                enemyAI.allAINodes = GameObject.FindGameObjectsWithTag("OutsideAINode");
                enemyAI.SyncPositionToClients();
            }

            RoundManager.Instance.SpawnedEnemies.Add(enemyAI);
            return enemyAI;
        }

        public static EnemyAI SpawnEnemyFromVent(Type EnemyType, bool ForceInside)
        {
            GameObject enemyPrefab = null;

            //Find enemy prefab by type
            if (ForceInside)
            {
                enemyPrefab = FindEnemyPrefabByType(EnemyType, RoundManager.Instance.currentLevel.OutsideEnemies, CurrentLevel);
            }
            else
            {
                enemyPrefab = FindEnemyPrefabByType(EnemyType, RoundManager.Instance.currentLevel.Enemies, CurrentLevel);
            }

            var ventIndex = Random.Range(0, RoundManager.Instance.allEnemyVents.Length);
            var spawnPosition = RoundManager.Instance.allEnemyVents[ventIndex].floorNode.position;
            var spawnRotation = Quaternion.Euler(0, RoundManager.Instance.allEnemyVents[ventIndex].floorNode.eulerAngles.y, 0);

            //Instantiate enemy at the vent location
            var enemyObject = Object.Instantiate(enemyPrefab, spawnPosition, spawnRotation);
            enemyObject.GetComponentInChildren<NetworkObject>().Spawn(true);

            //Configure the EnemyAI
            var enemyAI = enemyObject.GetComponent<EnemyAI>();

            if (ForceInside)
            {
                enemyAI.enemyType.isOutsideEnemy = false;
                enemyAI.allAINodes = GameObject.FindGameObjectsWithTag("AINode");
                enemyAI.SyncPositionToClients();
            }

            RoundManager.Instance.SpawnedEnemies.Add(enemyAI);
            return enemyAI;
        }

        public static void SpawnMultipleEnemies(List<EnemySpawnInfo> EnemiesToSpawn)
        {
            foreach (var enemyInfo in EnemiesToSpawn)
            {
                for (var i = 0; i < enemyInfo.Amount; i++)
                {
                    switch (enemyInfo.Location)
                    {
                        case SpawnLocation.Outside:
                            SpawnEnemyOutside(enemyInfo.EnemyType, enemyInfo.ForceOutside);
                            break;
                        case SpawnLocation.Vent:
                            SpawnEnemyFromVent(enemyInfo.EnemyType, enemyInfo.ForceInside);
                            break;
                    }
                }
            }
        }

        //Spawns MouthDogs Inside .-.
        public static void TheBeastsInside()
        {
            var levelName = CurrentLevel.sceneName;
            var spawnAmount = 0;
            DogForceOwnership = true;

            switch (levelName)
            {
                case "Titan":
                    spawnAmount = 6;
                    break;

                case "Rend":
                case "Dine":
                    spawnAmount = 4;
                    break;

                case "March":
                case "Offense":
                    spawnAmount = 3;
                    break;

                default:
                    spawnAmount = 2;
                    break;
            }
            PresetEnemiesToSpawn.Add(new EnemySpawnInfo(typeof(MouthDogAI), spawnAmount, SpawnLocation.Vent, true, false));
        }

        //Spawns Giants outside .-.
        public static void TheRumbling()
        {
            var levelName = CurrentLevel.sceneName;
            var spawnAmount = 0;

            switch (levelName)
            {
                case "Titan":
                case "March":
                case "Offense":
                case "Vow":
                    spawnAmount = 8;
                    break;

                case "Rend":
                case "Dine":
                    spawnAmount = 10;
                    break;

                case "Experimentation":
                    spawnAmount = 6;
                    break;

                default:
                    spawnAmount = 8;
                    break;
            }
            PresetEnemiesToSpawn.Add(new EnemySpawnInfo(typeof(ForestGiantAI), spawnAmount, SpawnLocation.Outside, false, false));
        }

        //public static void SpawnDogsInside(int amount)
        //{
        //    Variables.DogForceOwnership = true;
        //
        //    int DogIndex = 0; // Default Index except on paid maps
        //    for (int n = 0; n < RoundManager.Instance.currentLevel.OutsideEnemies.Count; n++)
        //    {
        //        var Enemy = RoundManager.Instance.currentLevel.OutsideEnemies[n];
        //        if (Enemy.enemyType.name == "MouthDog")
        //        {
        //            DogIndex = n;
        //            break;
        //        }
        //    }
        //
        //    List<int> availableVents = Enumerable.Range(0, RoundManager.Instance.allEnemyVents.Length).ToList();
        //    GameObject Dog = RoundManager.Instance.currentLevel.OutsideEnemies[DogIndex].enemyType.enemyPrefab;
        //
        //    for (int i = 0; i < amount; i++)
        //    {
        //        int ventIndex;
        //        if (availableVents.Count > 0)
        //        {
        //            // Randomly select from available vents
        //            ventIndex = availableVents[UnityEngine.Random.Range(0, availableVents.Count)];
        //            availableVents.Remove(ventIndex);
        //        }
        //        else
        //        {
        //            // If all vents have been used, select randomly from all vents
        //            ventIndex = UnityEngine.Random.Range(0, RoundManager.Instance.allEnemyVents.Length);
        //        }
        //
        //        Vector3 position = RoundManager.Instance.allEnemyVents[ventIndex].floorNode.position;
        //        GameObject DogSpawned = UnityEngine.Object.Instantiate(Dog, position, Quaternion.Euler(Vector3.zero));
        //        DogSpawned.GetComponentInChildren<NetworkObject>().Spawn(true);
        //        RoundManager.Instance.SpawnedEnemies.Add(DogSpawned.GetComponent<EnemyAI>());
        //        EnemyAI DogType = DogSpawned.GetComponent<EnemyAI>();
        //
        //        // Store unique identifier for patch use
        //        int dogInstanceId = DogSpawned.GetInstanceID();
        //        Variables.DogsSpawnedInside.Add(dogInstanceId);
        //
        //        DogType.enemyType.isOutsideEnemy = false;
        //        DogType.allAINodes = GameObject.FindGameObjectsWithTag("AINode");
        //        DogType.SyncPositionToClients();
        //    }
        //}

        //Spawns Springmen outside and collects instanceID to use in Harmony Patch
        public static void InsideOutEnemies()
        {
            InsideOutOwnership = true;

            for (var i = 0; i < 4; i++)
            {
                var spawnedSpring = SpawnEnemyOutside(typeof(SpringManAI), true);
                SpawnedInsideOutID.Add(spawnedSpring.GetInstanceID());
            }
        }

        public static void SetLightningStrikeInterval()
        {
            LightningStrikeInterval = Random.Range(0f, 10f);
        }

        //(testing) New event idea to force lightning to strike rapidly, though it does cause intense server lag
        public static void LightningStrikeRandom()
        {
            if (SmiteOutsideNodes == null || SmiteOutsideNodes.Length == 0)
            {
                Mls.LogError("Smite_outsideNodes is null or empty.");
                InitializeSmiteOutsideNodes(); // Attempt to initialize if not done already
                if (SmiteOutsideNodes == null || SmiteOutsideNodes.Length == 0)
                {
                    return;
                }
            }

            try
            {
                Vector3 vector;
                if (SmiteSeed.Next(0, 100) < 60 && (RandomThunderTime - TimeAtLastStrike) * TimeOfDay.Instance.currentWeatherVariable < 3f)
                {
                    vector = LastRandomStrikePosition;
                }
                else
                {
                    var num = SmiteSeed.Next(0, SmiteOutsideNodes.Length);
                    vector = SmiteOutsideNodes[num].transform.position;
                    vector = RoundManager.Instance.GetRandomNavMeshPositionInBoxPredictable(vector, 15f, SmiteNavHit, SmiteSeed);
                    LastRandomStrikePosition = vector;
                }

                if (StartOfRound.Instance.shipHasLanded)
                {
                    RoundManager.Instance.LightningStrikeServerRpc(vector);
                }
            }
            catch (Exception ex)
            {
                Mls.LogError($"Error in LightningStrikeRandom: {ex.Message}\nStack Trace: {ex.StackTrace}");
                BcpLogger.Log($"Error in LightningStrikeRandom: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }
        }

        public static void InitializeSmiteOutsideNodes()
        {
            SmiteOutsideNodes = GameObject.FindGameObjectsWithTag("OutsideAINode");

            if (SmiteOutsideNodes != null && SmiteOutsideNodes.Length > 0)
            {
                Mls.LogInfo($"Found {SmiteOutsideNodes.Length} OutsideAINode objects.");
            }
            else
            {
                Mls.LogError("No OutsideAINode objects found in the scene.");
            }
        }

        //Looped from Update() to spawn landmines under players feet if on planet surface (this could be tweaked)
        public static void SurfaceExplosionLoop()
        {
            if (SlSpawnTimer > 0f)
            {
                SlSpawnTimer = Random.Range(-4, -1);
                var allPlayerScripts = StartOfRound.Instance.allPlayerScripts;
                var playerControllerB = allPlayerScripts[Random.Range(0, allPlayerScripts.Length)];
                if (playerControllerB != null && !playerControllerB.isInHangarShipRoom && !playerControllerB.isInsideFactory && playerControllerB.isGroundedOnServer)
                {
                    if (Vector3.Distance(playerControllerB.transform.position, new Vector3(9.33f, 5.2f, 1021f)) < 1f)
                    {
                        SlSpawnTimer = 1f;
                        return;
                    }
                    var gameObject = Object.Instantiate(Variables.Landmine, playerControllerB.transform.position, Quaternion.identity);
                    gameObject.transform.position = playerControllerB.transform.position;
                    Mls.LogWarning(playerControllerB.transform.position);
                    gameObject.GetComponent<NetworkObject>().Spawn(true);
                    ObjectsToCleanUp.Add(gameObject);
                }
            }
            else
            {
                SlSpawnTimer += Time.deltaTime;
            }
        }

        //Looped from Update() to select a target to kill (this could be tweaked)
        public static void HungerGamesLoop()
        {
            //Change Tribute based on events
            if (SacrificeTargetChangeTimer >= TargetChangeInterval ||
                SacrificeTarget == null ||
                !SacrificeTarget.isPlayerControlled)
            {
                PickNewSacrificeTarget();
                SacrificeTargetChangeTimer = 0;
            }

            //Apply the effect if the target is inside the factory
            if (SacrificeTarget != null && SacrificeTarget.isInsideFactory)
            {
                ApplySacrificeEffect();
                Tribute = false;
            }
        }

        public static void PickNewSacrificeTarget()
        {
            var allPlayers = StartOfRound.Instance.allPlayerScripts;
            if (allPlayers.Length > 0)
            {
                SacrificeTarget = allPlayers[Random.Range(0, allPlayers.Length)];
            }
        }

        public static void ApplySacrificeEffect()
        {
            if (!SacrificeTarget.isPlayerDead)
            {
                SacrificeTarget.DamagePlayerFromOtherClientServerRpc(500, new Vector3(), (int)GameNetworkManager.Instance.localPlayerController.playerClientId);
                HUDManager.Instance.AddTextToChatOnServer($"<color=purple>{SacrificeTarget.playerUsername}</color> <color=orange>volunteered as Tribute!</color>");
            }
        }

        //Rebuilt Mouthdog function to align with The Beast Inside Function
        public static void EnterLunge(MouthDogAI Instance, Ray Ray, RaycastHit RayHit, RoundManager RoundManager)
        {
            Instance.SwitchToBehaviourState(3);
            Instance.endingLunge = false;
            Ray = new Ray(Instance.transform.position + Vector3.up, Instance.transform.forward);
            Vector3 vector;
            if (Physics.Raycast(Ray, out RayHit, 17f, StartOfRound.Instance.collidersAndRoomMask))
            {
                vector = RayHit.point;
            }
            else
            {
                vector = Ray.GetPoint(17f);
            }
            vector = RoundManager.GetNavMeshPosition(vector);
            Instance.SetDestinationToPosition(vector);
            Instance.agent.speed = 13f;
        }

        public static Vector3 GetNearbyLocation(Vector3 BaseLocation)
        {
            var offsetDistance = 10.0f;
            var offset = Random.insideUnitSphere * offsetDistance;
            return BaseLocation + offset;
        }

        //Custom function to allow Blobs to open doors, in theory this would work with any enemy that can't open doors
        public static void OpenDoors()
        {
            var doors = Object.FindObjectsOfType<DoorLock>();
            foreach (var door in doors)
            {
                var hitColliders = Physics.OverlapSphere(door.transform.position, 2f);
                var blobNearDoor = false;
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Enemy"))
                    {
                        blobNearDoor = true;
                        break;
                    }
                }

                if (blobNearDoor)
                {
                    var doorlock = typeof(DoorLock);
                    var isDoorOpen = doorlock.GetField("isDoorOpened", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (isDoorOpen == null)
                    {
                        return;
                    }

                    if (!door.isLocked && !(bool)(isDoorOpen.GetValue(door)))
                    {
                        door.OpenOrCloseDoor(null);
                    }
                }
            }
        }

        //Log the current level SpawnableEnemyWithRarity List
        public static void LogEnemyList(SelectableLevel Level)
        {
            if (Level != null && Level.Enemies.Count > 0)
            {
                foreach (var enemy in Level.Enemies)
                {
                    BcpLogger.Log($"{enemy.enemyType.enemyName} with a Rarity of {enemy.rarity} on {Level.sceneName}");
                }
            }
        }

        //Clone the current level SpawnableEnemyWithRarity List
        public static void CloneEnemyList(SelectableLevel Level)
        {
            if (Level != null && Level.Enemies.Count > 0)
            {
                foreach (var enemy in Level.Enemies)
                {
                    OriginalEnemyListWithRarity.Add(enemy);
                    BcpLogger.Log($"{enemy.enemyType.enemyName} with a Rarity of {enemy.rarity} on {Level.sceneName}");
                }
            }
        }

        //Reset the previous level back to its original state involving SpawnableEnemyWithRarity List
        public static void SetEnemyListOriginalState(SelectableLevel Level)
        {
            if (Level != null && OriginalEnemyListWithRarity.Count > 0)
            {
                Level.Enemies.Clear();
                foreach (var enemy in OriginalEnemyListWithRarity)
                {
                    Level.Enemies.Add(enemy);
                    BcpLogger.Log($"Reverting {enemy.enemyType.enemyName} with a Rarity of {enemy.rarity} on {Level.sceneName}");
                }
                OriginalEnemyListWithRarity.Clear();
            }
        }

        //Could prolly be better c:
        public static void CleanUpAllVariables()
        {
            DogForceOwnership = false;
            InsideOutOwnership = false;

            BlobsHaveEvolved = false;
            InstaJester = false;

            Variables.TheRumbling = false;

            SmiteEnabled = false;

            SacrificeTarget = null;
            Tribute = false;

            SpawnInsideOut = false;

            WaitUntilPlayerInside = false;

            Landed = false;

            SacrificeTargetChangeTimer = 0;
            SurpriseLandmines = -1;


            SpawnedInsideOutID.Clear();
            PresetEnemiesToSpawn.Clear();
            DogsSpawnedInside.Clear();


            foreach (var aiType in AIPresence.Keys.ToList())
            {
                AIPresence[aiType] = false;
            }
        }

    }
}
