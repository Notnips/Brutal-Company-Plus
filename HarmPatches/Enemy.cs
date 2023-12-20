using BrutalCompanyPlus.BCP;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Net.Cache;
using System.Reflection;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

namespace BrutalCompanyPlus.HarmPatches
{
    public static class EnemyAIPatches
    {

        //Force Blob to open doors and Speed up, Ownership always required
        [HarmonyPatch(typeof(BlobAI), "Update")]
        [HarmonyPostfix]
        public static void BlobModifications(BlobAI __instance)
        {
            if (__instance.isOutside)
            {
                if (__instance.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId)
                {
                    __instance.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
                }
            }

            if (Variables.BlobsHaveEvolved && RoundManager.Instance.IsHost)
            {
                if (__instance.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId)
                {
                    __instance.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
                }
                else
                {
                    __instance.agent.speed = 3.8f;
                    Functions.OpenDoors();
                }
            }
        }

        [HarmonyPatch(typeof(FlowermanAI), "Update")]
        [HarmonyPostfix]
        public static void FlowerManModifications(FlowermanAI __instance)
        {
            if (__instance.isOutside)
            {
                if (__instance.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId)
                {
                    __instance.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
                }
            }
        }

        [HarmonyPatch(typeof(HoarderBugAI), "Update")]
        [HarmonyPostfix]
        public static void HoarderBugModifications(HoarderBugAI __instance)
        {
            if (__instance.isOutside)
            {
                if (__instance.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId)
                {
                    __instance.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
                }
            }
        }

        public static bool HoldState = false;
        [HarmonyPatch(typeof(JesterAI), "Update")]
        [HarmonyPostfix]
        public static void InsideOutJesterModifications(JesterAI __instance)
        {
            if (RoundManager.Instance.IsHost)
            {
                if (__instance.isOutside)
                {
                    if (__instance.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId)
                    {
                        __instance.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
                    }
                    if (__instance.previousBehaviourStateIndex == 1 || HoldState)
                    {
                        __instance.SwitchToBehaviourState(2);
                        __instance.agent.stoppingDistance = 0;
                        HoldState = true;
                        bool PlayersAreAllInside = true;
                        for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++)
                        {
                            if (!StartOfRound.Instance.allPlayerScripts[i].isInsideFactory && StartOfRound.Instance.allPlayerScripts[i].isPlayerControlled)
                            {
                                PlayersAreAllInside = false;
                                break;
                            }
                        }
                        if (PlayersAreAllInside || StartOfRound.Instance.shipIsLeaving)
                        {
                            __instance.previousBehaviourStateIndex = 0;
                            __instance.agent.stoppingDistance = 4;
                            __instance.SwitchToBehaviourState(0);
                            HoldState = false;
                        }
                    }
                }
            }
        }

        //Force Ownership to prevent a client from crashing when the AI is outside
        [HarmonyPatch(typeof(SpringManAI), "Update")]
        [HarmonyPostfix]
        public static void InsideOutSpringManModifications(SpringManAI __instance)
        {
            if (__instance.isOutside)
            {
                if (__instance.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId)
                {
                    __instance.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
                }
            }
        }

        //Force SpringMan to search near the ship
        [HarmonyPatch(typeof(EnemyAI), "StartSearch")]
        [HarmonyPrefix]
        public static bool InsideOutStartSearchModification(EnemyAI __instance, ref Vector3 startOfSearch, AISearchRoutine newSearch)
        {
            if (Variables.InsideOutOwnership && RoundManager.Instance.IsHost)
            {
                if (Variables.SpawnedInsideOutID.Contains(__instance.GetInstanceID()))
                {
                    // Modify the startOfSearch to be near the ship's location
                    Vector3 shipLocation = StartOfRound.Instance.playerSpawnPositions[0].position;
                    startOfSearch = Functions.GetNearbyLocation(shipLocation);
                    Variables.mls.LogError($"Changed Search Start Location of Enemy insideOut");
                    return true;
                }
            }
            return true;
        }

        //Force Jester to pop out early, requires Ownership until popout occurs
        [HarmonyPatch(typeof(JesterAI), "Update")]
        [HarmonyPostfix]
        public static void InstaJesterModifications(JesterAI __instance)
        {
            if (Variables.InstaJester && RoundManager.Instance.IsHost)
            {
                if (__instance.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId && __instance.currentBehaviourStateIndex < 2)
                {
                    __instance.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
                }
                else
                {
                    if (__instance.currentBehaviourStateIndex == 1)
                    {
                        if (__instance.popUpTimer > 10f)
                        {
                            __instance.popUpTimer = UnityEngine.Random.Range(0f, 10f);
                        }
                    }
                }
            }
        }

        //Force giants to slow down, Ownership always required
        [HarmonyPatch(typeof(ForestGiantAI), "Update")]
        [HarmonyPostfix]
        public static void RumblingForestGiantModifications(ForestGiantAI __instance, bool ___inEatingPlayerAnimation)
        {
            if (Variables.TheRumbling && RoundManager.Instance.IsHost)
            {
                if (__instance.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId && __instance.currentBehaviourStateIndex < 2)
                {
                    __instance.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
                }
                else
                {
                    if (__instance.currentBehaviourStateIndex == 1)
                    {
                        if (!___inEatingPlayerAnimation)
                        {
                            __instance.agent.speed = 6f;
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
        public static void InsideMouthDogModifications(MouthDogAI __instance)
        {
            if (Variables.DogForceOwnership && RoundManager.Instance.IsHost)
            {
                if (__instance.OwnerClientId != GameNetworkManager.Instance.localPlayerController.actualClientId)
                {
                    __instance.ChangeOwnershipOfEnemy(GameNetworkManager.Instance.localPlayerController.actualClientId);
                }
            }
        }

        //Function Overwrite to allow Dogs to kill other players while inside factory, cause why not add code to prevent dogs killing players in a factory .----.
        [HarmonyPatch(typeof(MouthDogAI), "OnCollideWithPlayer")]
        [HarmonyPrefix]
        public static bool MouthDogAIOnCollideWithPlayerPatch(MouthDogAI __instance, ref Collider other, bool ___inKillAnimation, Collider ___debugCollider, bool ___inLunge, Ray ___ray, RaycastHit ___rayHit, RoundManager ___roundManager)
        {
            if (Variables.DogForceOwnership && RoundManager.Instance.IsHost)
            {

                //Variables.mls.LogWarning($"\n\n\n\n\n\n\n\n\n\n\n-------------- OnCollideWithPlayer Function Hit Detection --------------\n\n\n\n\n\n\n\n\n\n\n");

                PlayerControllerB target = other.gameObject.GetComponent<PlayerControllerB>();
                if (target != null && !target.isPlayerDead && target.isPlayerControlled && !___inKillAnimation)
                {
                    Vector3 a = Vector3.Normalize((__instance.transform.position + Vector3.up - target.gameplayCamera.transform.position) * 100f);
                    RaycastHit raycastHit;
                    if (Physics.Linecast(__instance.transform.position + Vector3.up + a * 0.5f, target.gameplayCamera.transform.position, out raycastHit, StartOfRound.Instance.collidersAndRoomMask, QueryTriggerInteraction.Ignore))
                    {
                        if (raycastHit.collider == ___debugCollider)
                        {
                            //Variables.mls.LogWarning($"\n\n\n\n\n\n\n\n\n\n\n-------------- OnCollideWithPlayer Function Returning False #1 --------------\n\n\n\n\n\n\n\n\n\n\n");
                            return false;
                        }
                        ___debugCollider = raycastHit.collider;
                        //Variables.mls.LogWarning($"\n\n\n\n\n\n\n\n\n\n\n-------------- OnCollideWithPlayer Function Returning False #2 --------------\n\n\n\n\n\n\n\n\n\n\n");
                        return false;
                    }
                    else
                    {
                        if (__instance.currentBehaviourStateIndex == 3)
                        {
                            target.inAnimationWithEnemy = __instance;
                            __instance.KillPlayerServerRpc((int)target.playerClientId);
                            //Variables.mls.LogWarning($"\n\n\n\n\n\n\n\n\n\n\n-------------- ATTEMPTING TO KILL {target.playerUsername} WITH DOG COLLIDER  --------------\n{target.playerUsername} {target.inAnimationWithEnemy}\n\n\n\n\n\n\n\n\n\n\n");
                            return false;
                        }
                        if (__instance.currentBehaviourStateIndex == 0 || __instance.currentBehaviourStateIndex == 1)
                        {
                            __instance.SwitchToBehaviourState(2);
                            __instance.SetDestinationToPosition(target.transform.position, false);
                            //Variables.mls.LogWarning($"\n\n\n\n\n\n\n\n\n\n\n-------------- SetDestinationToPosition Function Returning False --------------\n\n\n\n\n\n\n\n\n\n\n");
                            return false;
                        }
                        if (__instance.currentBehaviourStateIndex == 2 && !___inLunge)
                        {
                            __instance.transform.LookAt(other.transform.position);
                            __instance.transform.localEulerAngles = new Vector3(0f, __instance.transform.eulerAngles.y, 0f);
                            ___inLunge = true;
                            Functions.EnterLunge(__instance, ___ray, ___rayHit, ___roundManager);
                            //Variables.mls.LogWarning($"\n\n\n\n\n\n\n\n\n\n\n-------------- EnterLunge Function Returning False --------------\n\n\n\n\n\n\n\n\n\n\n");
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        //Function Overwrite to allow Braken to kill other players while outside the factory .----.
        [HarmonyPatch(typeof(FlowermanAI), "OnCollideWithPlayer")]
        [HarmonyPostfix]
        public static void FlowerManAIOnCollideWithPlayerPatch(FlowermanAI __instance, ref Collider other, bool ___startingKillAnimationLocalClient)
        {
            if (true)
            {
                PlayerControllerB target = other.gameObject.GetComponent<PlayerControllerB>();
        
                if (target != null && !target.isPlayerDead && target.isPlayerControlled)
                {
                    if (!__instance.inKillAnimation && !__instance.isEnemyDead && !___startingKillAnimationLocalClient)
                    {
                        __instance.KillPlayerAnimationServerRpc((int)target.playerClientId);
                        ___startingKillAnimationLocalClient = true;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(JesterAI), "OnCollideWithPlayer")]
        [HarmonyPostfix]
        public static void JesterAIOnCollideWithPlayerPatch(JesterAI __instance, ref Collider other, bool ___inKillAnimation)
        {
            if (true)
            {
                PlayerControllerB target = other.gameObject.GetComponent<PlayerControllerB>();
        
                if (target != null && !target.isPlayerDead && target.isPlayerControlled)
                {
                    if (!___inKillAnimation && !__instance.isEnemyDead && __instance.currentBehaviourStateIndex == 2)
                    {
                        __instance.KillPlayerServerRpc((int)target.playerClientId);
                        ___inKillAnimation = true;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(BlobAI), "OnCollideWithPlayer")]
        [HarmonyPostfix]
        public static void BlobAIOnCollideWithPlayerPatch(BlobAI __instance, ref Collider other)
        {
            if (true)
            {
                PlayerControllerB target = other.gameObject.GetComponent<PlayerControllerB>();
        
                if (target != null && !target.isPlayerDead && target.isPlayerControlled)
                {
                    Type blobinfo = typeof(BlobAI);
                    FieldInfo timeSinceHittingLocalPlayer = blobinfo.GetField("timeSinceHittingLocalPlayer", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo tamedTimer = blobinfo.GetField("tamedTimer", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo angeredTimer = blobinfo.GetField("angeredTimer", BindingFlags.NonPublic | BindingFlags.Instance);
                    if ((float)timeSinceHittingLocalPlayer.GetValue(__instance) < 0.25f)
                    {
                        return;
                    }
                    if ((float)tamedTimer.GetValue(__instance) > 0f && (float)angeredTimer.GetValue(__instance) < 0f)
                    {
                        return;
                    }
                    timeSinceHittingLocalPlayer.SetValue(__instance, 0);
                    target.DamagePlayer(35, true, true, CauseOfDeath.Unknown, 0, false, default(Vector3));
                    if (target.isPlayerDead)
                    {
                        __instance.SlimeKillPlayerEffectServerRpc((int)target.playerClientId);
                    }
                }
            }
        }


        [HarmonyPatch(typeof(HoarderBugAI), "OnCollideWithPlayer")]
        [HarmonyPostfix]
        public static void HoarderBugAIOnCollideWithPlayerPatch(HoarderBugAI __instance, ref Collider other)
        {
            if (true)
            {
                PlayerControllerB target = other.gameObject.GetComponent<PlayerControllerB>();
        
                if (target != null && !target.isPlayerDead && target.isPlayerControlled)
                {
                    Type hoarderinfo = typeof(HoarderBugAI);
                    FieldInfo timeSinceHittingPlayer = hoarderinfo.GetField("timeSinceHittingPlayer", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo inChase = hoarderinfo.GetField("inChase", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (!(bool)inChase.GetValue(__instance))
                    {
                        return;
                    }
                    if ((float)timeSinceHittingPlayer.GetValue(__instance) < 0.5f)
                    {
                        return;
                    }
                    timeSinceHittingPlayer.SetValue(__instance, 0);
                    target.DamagePlayer(30, true, true, CauseOfDeath.Mauling, 0, false, default(Vector3));
                    if (target.isPlayerDead)
                    {
                        __instance.HitPlayerServerRpc();
                    }
                }
            }
        }
    }
}
