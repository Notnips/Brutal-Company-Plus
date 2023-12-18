// ReSharper disable InconsistentNaming,RedundantAssignment

using BrutalCompanyPlus.BCP;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace BrutalCompanyPlus.Patches; 

[HarmonyPatch]
public static class EnemyAIPatches {
    //Force Blob to open doors and Speed up, Ownership always required
    [HarmonyPatch(typeof(BlobAI), "Update")]
    [HarmonyPostfix]
    public static void BlobModifications(BlobAI Instance) {
        if (Variables.BlobsHaveEvolved && RoundManager.Instance.IsHost) {
            if (Instance.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId) {
                Instance.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
            } else {
                Instance.agent.speed = 3.8f;
                Functions.OpenDoors();
            }
        }
    }

    //[HarmonyPatch(typeof(JesterAI), "Update")]
    //[HarmonyPostfix]
    //public static void InsideOutJesterModifications(JesterAI __instance)
    //{
    //    if (Variables.InsideOutOwnership && RoundManager.Instance.IsHost)
    //    {
    //        // Check if the instance ID is in the HashSet
    //        if (Variables.SpawnedInsideOutID.Contains(__instance.GetInstanceID()))
    //        {
    //            // If the instance is owned by someone other than the local player
    //            if (__instance.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId)
    //            {
    //                __instance.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
    //            }
    //        }
    //    }
    //}

    //Force Ownership to prevent a client from crashing when the AI is outside
    [HarmonyPatch(typeof(SpringManAI), "Update")]
    [HarmonyPostfix]
    public static void InsideOutSpringManModifications(SpringManAI Instance) {
        if (Variables.InsideOutOwnership && RoundManager.Instance.IsHost) {
            if (Variables.SpawnedInsideOutID.Contains(Instance.GetInstanceID())) {
                if (Instance.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId) {
                    Instance.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
                }
            }
        }
    }

    //Force SpringMan to search near the ship
    [HarmonyPatch(typeof(EnemyAI), "StartSearch")]
    [HarmonyPrefix]
    public static bool InsideOutStartSearchModification(EnemyAI Instance, ref Vector3 StartOfSearch,
        AISearchRoutine NewSearch) {
        if (Variables.InsideOutOwnership && RoundManager.Instance.IsHost) {
            if (Variables.SpawnedInsideOutID.Contains(Instance.GetInstanceID())) {
                // Modify the startOfSearch to be near the ship's location
                var shipLocation = StartOfRound.Instance.playerSpawnPositions[0].position;
                StartOfSearch = Functions.GetNearbyLocation(shipLocation);
                Variables.Mls.LogError("Changed Search Start Location of Enemy insideOut");
                return true;
            }
        }

        return true;
    }

    //Force Jester to pop out early, requires Ownership until popout occurs
    [HarmonyPatch(typeof(JesterAI), "Update")]
    [HarmonyPostfix]
    public static void InstaJesterModifications(JesterAI Instance) {
        if (Variables.InstaJester && RoundManager.Instance.IsHost) {
            if (Instance.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId &&
                Instance.currentBehaviourStateIndex < 2) {
                Instance.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
            } else {
                if (Instance.currentBehaviourStateIndex == 1) {
                    if (Instance.popUpTimer > 10f) {
                        Instance.popUpTimer = Random.Range(0f, 10f);
                    }
                }
            }
        }
    }

    //Force giants to slow down, Ownership always required
    [HarmonyPatch(typeof(ForestGiantAI), "Update")]
    [HarmonyPostfix]
    public static void RumblingForestGiantModifications(ForestGiantAI Instance, bool InEatingPlayerAnimation) {
        if (Variables.TheRumbling && RoundManager.Instance.IsHost) {
            if (Instance.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId &&
                Instance.currentBehaviourStateIndex < 2) {
                Instance.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
            } else {
                if (Instance.currentBehaviourStateIndex == 1) {
                    if (!InEatingPlayerAnimation) {
                        Instance.agent.speed = 6f;
                    }
                }
            }
        }
    }

    //[HarmonyPatch(typeof(EnemyAI), "ChangeOwnershipOfEnemy")]
    //[HarmonyPrefix]
    //public static bool ChangeOwnershipOfEnemyPatch(EnemyAI __instance)
    //{
    //    //Prevent dogs we spawned inside from getting ownership changed when passive
    //    if (__instance.enemyType == RoundManager.Instance.currentLevel.OutsideEnemies[(int)OutsideEnemyEnum.MouthDog].enemyType)
    //    {
    //        Variables.mls.LogWarning($"\n\n\n\n\n\n\n\n\n\n\n-------------- Preventing Dog Ownership Change --------------\n\n\n\n\n\n\n\n\n\n\n");
    //        return false;
    //    }
    //
    //    return true;
    //}

    //Force Ownership to prevent a client from crashing when the AI is inside
    [HarmonyPatch(typeof(MouthDogAI), "Update")]
    [HarmonyPostfix]
    public static void InsideMouthDogModifications(MouthDogAI Instance) {
        if (Variables.DogForceOwnership && RoundManager.Instance.IsHost) {
            if (Instance.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId) {
                Instance.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
            }
        }
    }

    //Function Overwrite to allow Dogs to kill other players while inside factory, cause why not add code to prevent dogs killing players in a factory .----.
    [HarmonyPatch(typeof(MouthDogAI), "OnCollideWithPlayer")]
    [HarmonyPrefix]
    public static bool OnCollideWithPlayerPatch(MouthDogAI Instance, ref Collider Other, bool InKillAnimation,
        Collider DebugCollider, bool InLunge, Ray Ray, RaycastHit RayHit, RoundManager RoundManager) {
        if (Variables.DogForceOwnership && RoundManager.Instance.IsHost) {
            //Variables.mls.LogWarning($"\n\n\n\n\n\n\n\n\n\n\n-------------- OnCollideWithPlayer Function Hit Detection --------------\n\n\n\n\n\n\n\n\n\n\n");

            var target = Other.gameObject.GetComponent<PlayerControllerB>();
            if (target != null && !target.isPlayerDead && target.isPlayerControlled && !InKillAnimation) {
                var a = Vector3.Normalize((Instance.transform.position + Vector3.up -
                                           target.gameplayCamera.transform.position) * 100f);
                RaycastHit raycastHit;
                if (Physics.Linecast(Instance.transform.position + Vector3.up + a * 0.5f,
                        target.gameplayCamera.transform.position, out raycastHit,
                        StartOfRound.Instance.collidersAndRoomMask, QueryTriggerInteraction.Ignore)) {
                    if (raycastHit.collider == DebugCollider) {
                        //Variables.mls.LogWarning($"\n\n\n\n\n\n\n\n\n\n\n-------------- OnCollideWithPlayer Function Returning False #1 --------------\n\n\n\n\n\n\n\n\n\n\n");
                        return false;
                    }

                    DebugCollider = raycastHit.collider;
                    //Variables.mls.LogWarning($"\n\n\n\n\n\n\n\n\n\n\n-------------- OnCollideWithPlayer Function Returning False #2 --------------\n\n\n\n\n\n\n\n\n\n\n");
                    return false;
                }

                if (Instance.currentBehaviourStateIndex == 3) {
                    target.inAnimationWithEnemy = Instance;
                    Instance.KillPlayerServerRpc((int)target.playerClientId);
                    //Variables.mls.LogWarning($"\n\n\n\n\n\n\n\n\n\n\n-------------- ATTEMPTING TO KILL {target.playerUsername} WITH DOG COLLIDER  --------------\n{target.playerUsername} {target.inAnimationWithEnemy}\n\n\n\n\n\n\n\n\n\n\n");
                    return false;
                }

                if (Instance.currentBehaviourStateIndex == 0 || Instance.currentBehaviourStateIndex == 1) {
                    Instance.SwitchToBehaviourState(2);
                    Instance.SetDestinationToPosition(target.transform.position);
                    //Variables.mls.LogWarning($"\n\n\n\n\n\n\n\n\n\n\n-------------- SetDestinationToPosition Function Returning False --------------\n\n\n\n\n\n\n\n\n\n\n");
                    return false;
                }

                if (Instance.currentBehaviourStateIndex == 2 && !InLunge) {
                    Instance.transform.LookAt(Other.transform.position);
                    Instance.transform.localEulerAngles = new Vector3(0f, Instance.transform.eulerAngles.y, 0f);
                    InLunge = true;
                    Functions.EnterLunge(Instance, Ray, RayHit, RoundManager);
                    //Variables.mls.LogWarning($"\n\n\n\n\n\n\n\n\n\n\n-------------- EnterLunge Function Returning False --------------\n\n\n\n\n\n\n\n\n\n\n");
                    return false;
                }
            }
        }

        return true;
    }
}