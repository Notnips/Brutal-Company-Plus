using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using static BrutalCompanyPlus.BCP.Variables;

namespace BrutalCompanyPlus.BCP
{
    public static class Functions
    {
        public static void AddSpecificEnemiesForEvent(SelectableLevel newLevel, List<Type> enemyAITypes)
        {
            SelectableLevel[] levels = StartOfRound.Instance.levels;
            foreach (var level in levels)
            {
                foreach (var spawnable in level.Enemies)
                {
                    var enemyPrefab = spawnable.enemyType.enemyPrefab;

                    foreach (var aiType in enemyAITypes)
                    {
                        //Check if this enemy has the AI component we're looking for
                        if (enemyPrefab.GetComponent(aiType) != null)
                        {
                            //Check if this enemy is not already in the new level
                            if (!newLevel.Enemies.Any(e => e.enemyType.enemyPrefab == enemyPrefab))
                            {
                                newLevel.Enemies.Add(spawnable);
                                Variables.mls.LogInfo($"Added specific Enemy: > {spawnable.enemyType.enemyPrefab.name} < for event");
                                //BcpLogger.Log($"Added specific Enemy: > {spawnable.enemyType.enemyPrefab.name} < for event");
                            }
                        }
                    }
                }
            }
        }

        //This is designed to check for Enemy Type existance and add it to the Enemy list if it doesn't exist
        public static GameObject FindEnemyPrefabByType(Type enemyType, List<SpawnableEnemyWithRarity> enemyList, SelectableLevel newLevel)
        {
            foreach (var enemy in enemyList)
            {
                if (enemy.enemyType.enemyPrefab.GetComponent(enemyType) != null)
                {
                    return enemy.enemyType.enemyPrefab;
                }
            }

            //If the enemy type is not found, try to add it to the newLevel
            AddSpecificEnemiesForEvent(newLevel, new List<Type> { enemyType });

            //Search again in the newLevel's enemy list
            foreach (var enemy in newLevel.Enemies)
            {
                if (enemy.enemyType.enemyPrefab.GetComponent(enemyType) != null)
                {
                    return enemy.enemyType.enemyPrefab;
                }
            }

            throw new Exception($"Enemy type {enemyType.Name} not found and could not be added.");
        }

        public static EnemyAI SpawnEnemyOutside(Type enemyType, bool ForceOutside)
        {

            GameObject enemyPrefab = null;

            //Find enemy prefab by type
            if (ForceOutside)
            {
                enemyPrefab = FindEnemyPrefabByType(enemyType, RoundManager.Instance.currentLevel.Enemies, Variables.CurrentLevel);
            }
            else
            {
                enemyPrefab = FindEnemyPrefabByType(enemyType, RoundManager.Instance.currentLevel.OutsideEnemies, Variables.CurrentLevel);
            }
           

            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("OutsideAINode");
            Vector3 spawnPosition = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;

            //Instantiate enemy
            GameObject enemyObject = UnityEngine.Object.Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemyObject.GetComponentInChildren<NetworkObject>().Spawn(true);

            EnemyAI enemyAI = enemyObject.GetComponent<EnemyAI>();

            if (ForceOutside)
            {
                enemyAI.enemyType.isOutsideEnemy = true;
                enemyAI.allAINodes = GameObject.FindGameObjectsWithTag("OutsideAINode");
                enemyAI.SyncPositionToClients();
            }

            RoundManager.Instance.SpawnedEnemies.Add(enemyAI);
            return enemyAI;
        }

        public static EnemyAI SpawnEnemyFromVent(Type enemyType, bool ForceInside)
        {
            GameObject enemyPrefab = null;

            //Find enemy prefab by type
            if (ForceInside)
            {
                enemyPrefab = FindEnemyPrefabByType(enemyType, RoundManager.Instance.currentLevel.OutsideEnemies, Variables.CurrentLevel);
            }
            else
            {
                enemyPrefab = FindEnemyPrefabByType(enemyType, RoundManager.Instance.currentLevel.Enemies, Variables.CurrentLevel);
            }

            int ventIndex = UnityEngine.Random.Range(0, RoundManager.Instance.allEnemyVents.Length);
            Vector3 spawnPosition = RoundManager.Instance.allEnemyVents[ventIndex].floorNode.position;
            Quaternion spawnRotation = Quaternion.Euler(0, RoundManager.Instance.allEnemyVents[ventIndex].floorNode.eulerAngles.y, 0);

            //Instantiate enemy at the vent location
            GameObject enemyObject = UnityEngine.Object.Instantiate(enemyPrefab, spawnPosition, spawnRotation);
            enemyObject.GetComponentInChildren<NetworkObject>().Spawn(true);

            //Configure the EnemyAI
            EnemyAI enemyAI = enemyObject.GetComponent<EnemyAI>();

            if (ForceInside)
            {
                enemyAI.enemyType.isOutsideEnemy = false;
                enemyAI.allAINodes = GameObject.FindGameObjectsWithTag("AINode");
                enemyAI.SyncPositionToClients();
            }

            RoundManager.Instance.SpawnedEnemies.Add(enemyAI);
            return enemyAI;
        }

        public static void SpawnMultipleEnemies(List<EnemySpawnInfo> enemiesToSpawn)
        {
            foreach (var enemyInfo in enemiesToSpawn)
            {
                for (int i = 0; i < enemyInfo.Amount; i++)
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
            string LevelName = Variables.CurrentLevel.sceneName;
            int SpawnAmount = 0;
            Variables.DogForceOwnership = true;

            switch (LevelName)
            {
                case "Titan":
                    SpawnAmount = 6;
                    break;

                case "Rend":
                case "Dine":
                    SpawnAmount = 4;
                    break;

                case "March":
                case "Offense":
                    SpawnAmount = 3;
                    break;

                default:
                    SpawnAmount = 2;
                    break;
            }
            Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(MouthDogAI), SpawnAmount, SpawnLocation.Vent, true, false));
        }

        //Spawns Giants outside .-.
        public static void TheRumbling()
        {
            string LevelName = Variables.CurrentLevel.sceneName;
            int SpawnAmount = 0;

            switch (LevelName)
            {
                case "Titan":
                case "March":
                case "Offense":
                case "Vow":
                    SpawnAmount = 8;
                    break;

                case "Rend":
                case "Dine":
                    SpawnAmount = 10;
                    break;

                case "Experimentation":
                    SpawnAmount = 6;
                    break;

                default:
                    SpawnAmount = 8;
                    break;
            }
            Variables.presetEnemiesToSpawn.Add(new Variables.EnemySpawnInfo(typeof(ForestGiantAI), SpawnAmount, SpawnLocation.Outside, false, false));
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
            Variables.InsideOutOwnership = true;

            for (int i = 0; i < 4; i++)
            {
                EnemyAI SpawnedSpring = SpawnEnemyOutside(typeof(SpringManAI), true);
                Variables.SpawnedInsideOutID.Add(SpawnedSpring.GetInstanceID());
            }
        }

        public static void SetLightningStrikeInterval()
        {
            Variables.lightningStrikeInterval = UnityEngine.Random.Range(0f, 10f);
        }

        //(testing) New event idea to force lightning to strike rapidly, though it does cause intense server lag
        public static void LightningStrikeRandom()
        {
            if (Variables.Smite_outsideNodes == null || Variables.Smite_outsideNodes.Length == 0)
            {
                Variables.mls.LogError("Smite_outsideNodes is null or empty.");
                InitializeSmiteOutsideNodes(); // Attempt to initialize if not done already
                if (Variables.Smite_outsideNodes == null || Variables.Smite_outsideNodes.Length == 0)
                {
                    return;
                }
            }

            try
            {
                Vector3 vector;
                if (Variables.Smite_seed.Next(0, 100) < 60 && (Variables.randomThunderTime - Variables.timeAtLastStrike) * (float)TimeOfDay.Instance.currentWeatherVariable < 3f)
                {
                    vector = Variables.lastRandomStrikePosition;
                }
                else
                {
                    int num = Variables.Smite_seed.Next(0, Variables.Smite_outsideNodes.Length);
                    vector = Variables.Smite_outsideNodes[num].transform.position;
                    vector = RoundManager.Instance.GetRandomNavMeshPositionInBoxPredictable(vector, 15f, Variables.Smite_navHit, Variables.Smite_seed, -1);
                    Variables.lastRandomStrikePosition = vector;
                }

                if (StartOfRound.Instance.shipHasLanded)
                {
                    RoundManager.Instance.LightningStrikeServerRpc(vector);
                }
            }
            catch (Exception ex)
            {
                Variables.mls.LogError($"Error in LightningStrikeRandom: {ex.Message}\nStack Trace: {ex.StackTrace}");
                BcpLogger.Log($"Error in LightningStrikeRandom: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }
        }

        public static void InitializeSmiteOutsideNodes()
        {
            Variables.Smite_outsideNodes = GameObject.FindGameObjectsWithTag("OutsideAINode");

            if (Variables.Smite_outsideNodes != null && Variables.Smite_outsideNodes.Length > 0)
            {
                Variables.mls.LogInfo($"Found {Variables.Smite_outsideNodes.Length} OutsideAINode objects.");
            }
            else
            {
                Variables.mls.LogError("No OutsideAINode objects found in the scene.");
            }
        }

        //Looped from Update() to spawn landmines under players feet if on planet surface (this could be tweaked)
        public static void SurfaceExplosionLoop()
        {
            if (Variables.slSpawnTimer > 0f)
            {
                Variables.slSpawnTimer = (float)UnityEngine.Random.Range(-4, -1);
                PlayerControllerB[] allPlayerScripts = StartOfRound.Instance.allPlayerScripts;
                PlayerControllerB playerControllerB = allPlayerScripts[UnityEngine.Random.Range(0, allPlayerScripts.Length)];
                if (playerControllerB != null && !playerControllerB.isInHangarShipRoom && !playerControllerB.isInsideFactory)
                {
                    if (Vector3.Distance(playerControllerB.transform.position, new Vector3(9.33f, 5.2f, 1021f)) < 1f)
                    {
                        Variables.slSpawnTimer = 1f;
                        return;
                    }
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Variables.landmine, playerControllerB.transform.position, Quaternion.identity);
                    gameObject.transform.position = playerControllerB.transform.position;
                    Variables.mls.LogWarning(playerControllerB.transform.position);
                    gameObject.GetComponent<NetworkObject>().Spawn(true);
                    Variables.objectsToCleanUp.Add(gameObject);
                }
            }
            else
            {
                Variables.slSpawnTimer += Time.deltaTime;
            }
        }

        //Looped from Update() to select a target to kill (this could be tweaked)
        public static void HungerGamesLoop()
        {
            //Change Tribute based on events
            if (Variables.sacrificeTargetChangeTimer >= Variables.TargetChangeInterval ||
                Variables.SacrificeTarget == null ||
                !Variables.SacrificeTarget.isPlayerControlled)
            {
                Functions.PickNewSacrificeTarget();
                Variables.sacrificeTargetChangeTimer = 0;
            }

            //Apply the effect if the target is inside the factory
            if (Variables.SacrificeTarget != null && Variables.SacrificeTarget.isInsideFactory)
            {
                Functions.ApplySacrificeEffect();
                Variables.Tribute = false;
            }
        }

        public static void PickNewSacrificeTarget()
        {
            PlayerControllerB[] allPlayers = StartOfRound.Instance.allPlayerScripts;
            if (allPlayers.Length > 0)
            {
                Variables.SacrificeTarget = allPlayers[UnityEngine.Random.Range(0, allPlayers.Length)];
            }
        }

        public static void ApplySacrificeEffect()
        {
            if (!Variables.SacrificeTarget.isPlayerDead)
            {
                HUDManager.Instance.AddTextToChatOnServer($"<color=purple>{Variables.SacrificeTarget.playerUsername}</color> <color=orange>volunteered as Tribute!</color>", -1);
                Variables.SacrificeTarget.DamagePlayerFromOtherClientServerRpc(500, new Vector3(), (int)GameNetworkManager.Instance.localPlayerController.playerClientId);
            }
        }

        //Rebuilt Mouthdog function to align with The Beast Inside Function
        public static void EnterLunge(MouthDogAI __instance, Ray ray, RaycastHit rayHit, RoundManager roundManager)
        {
            __instance.SwitchToBehaviourState(3);
            __instance.endingLunge = false;
            ray = new Ray(__instance.transform.position + Vector3.up, __instance.transform.forward);
            Vector3 vector;
            if (Physics.Raycast(ray, out rayHit, 17f, StartOfRound.Instance.collidersAndRoomMask))
            {
                vector = rayHit.point;
            }
            else
            {
                vector = ray.GetPoint(17f);
            }
            vector = roundManager.GetNavMeshPosition(vector, default(NavMeshHit), 5f, -1);
            __instance.SetDestinationToPosition(vector, false);
            __instance.agent.speed = 13f;
        }

        public static Vector3 GetNearbyLocation(Vector3 baseLocation)
        {
            float offsetDistance = 10.0f;
            Vector3 offset = UnityEngine.Random.insideUnitSphere * offsetDistance;
            return baseLocation + offset;
        }

        //Custom function to allow Blobs to open doors, in theory this would work with any enemy that can't open doors
        public static void OpenDoors()
        {
            DoorLock[] doors = UnityEngine.Object.FindObjectsOfType<DoorLock>();
            foreach (var door in doors)
            {
                Collider[] hitColliders = Physics.OverlapSphere(door.transform.position, 2f);
                bool blobNearDoor = false;
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
                    Type doorlock = typeof(DoorLock);
                    FieldInfo isDoorOpen = doorlock.GetField("isDoorOpened", BindingFlags.NonPublic | BindingFlags.Instance);
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
        public static void LogEnemyList(SelectableLevel level)
        {
            if (level != null && level.Enemies.Count > 0)
            {
                foreach (var enemy in level.Enemies)
                {
                    BcpLogger.Log($"{enemy.enemyType.enemyName} with a Rarity of {enemy.rarity} on {level.sceneName}");
                }
            }
        }

        //Clone the current level SpawnableEnemyWithRarity List
        public static void CloneEnemyList(SelectableLevel level)
        {
            if (level != null && level.Enemies.Count > 0)
            {
                foreach (var enemy in level.Enemies)
                {
                    Variables.OriginalEnemyListWithRarity.Add(enemy);
                    BcpLogger.Log($"{enemy.enemyType.enemyName} with a Rarity of {enemy.rarity} on {level.sceneName}");
                }
            }
        }

        //Reset the previous level back to its original state involving SpawnableEnemyWithRarity List
        public static void SetEnemyListOriginalState(SelectableLevel level)
        {
            if (level != null && Variables.OriginalEnemyListWithRarity.Count > 0)
            {
                level.Enemies.Clear();
                foreach (var enemy in Variables.OriginalEnemyListWithRarity)
                {
                    level.Enemies.Add(enemy);
                    BcpLogger.Log($"Reverting {enemy.enemyType.enemyName} with a Rarity of {enemy.rarity} on {level.sceneName}");
                }
                Variables.OriginalEnemyListWithRarity.Clear();
            }
        }

        //Could prolly be better c:
        public static void CleanUpAllVariables()
        {
            Variables.DogForceOwnership = false;
            Variables.InsideOutOwnership = false;

            Variables.BlobsHaveEvolved = false;
            Variables.InstaJester = false;

            Variables.TheRumbling = false;

            Variables.SmiteEnabled = false;

            Variables.SacrificeTarget = null;
            Variables.Tribute = false;

            Variables.SpawnInsideOut = false;

            Variables.WaitUntilPlayerInside = false;

            Variables.Landed = false;

            Variables.sacrificeTargetChangeTimer = 0;
            Variables.surpriseLandmines = -1;


            Variables.SpawnedInsideOutID.Clear();
            Variables.presetEnemiesToSpawn.Clear();
            Variables.DogsSpawnedInside.Clear();


            foreach (var aiType in Variables.aiPresence.Keys.ToList())
            {
                Variables.aiPresence[aiType] = false;
            }
        }

    }
}
