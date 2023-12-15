using System;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyPlus.BCP
{
    public class BrutalPlus : MonoBehaviour
    {
        public void Awake()
        {
            Variables.mls.LogWarning("Brutal Plus is Awake");
        }

        public void Start()
        {
            BcpLogger.Start();
        }

        public void OnDestroy()
        {
            BcpLogger.Close();
        }

        void Update()
        {
            try
            {
                if (StartOfRound.Instance != null && RoundManager.Instance.IsHost)
                {
                    //Set Smite Effects
                    if (Variables.SmiteEnabled)
                    {
                        Functions.SetLightningStrikeInterval();

                        if (Time.time - Variables.lastStrikeTime > Variables.lightningStrikeInterval)
                        {
                            Functions.LightningStrikeRandom();
                            Variables.lastStrikeTime = Time.time;
                        }
                    }

                    //Set Hunger Games Event
                    if (Variables.Tribute)
                    {
                        Functions.HungerGamesLoop();
                    }

                    //Set Surface Explosion Event
                    if (Variables.surpriseLandmines > 0 && Variables.Landed)
                    {
                        Functions.SurfaceExplosionLoop();
                    }

                    //Set Turret in Ship Event
                    if (Variables.shouldSpawnTurret & Variables.turret != null)
                    {
                        Variables.shouldSpawnTurret = false;
                        Variables.TurretSpawned = true;
                        GameObject ShipTurret = UnityEngine.Object.Instantiate<GameObject>(Variables.turret, new Vector3(3.87f, 0.84f, -14.23f), Quaternion.identity);
                        ShipTurret.transform.position = new Vector3(3.87f, 0.84f, -14.23f);
                        ShipTurret.transform.forward = new Vector3(1f, 0f, 0f);
                        ShipTurret.GetComponent<NetworkObject>().Spawn(true);
                        Variables.objectsToCleanUp.Add(ShipTurret);
                    }

                    if (Variables.presetEnemiesToSpawn.Count > 0 && Variables.WaitUntilPlayerInside)
                    {
                        for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++)
                        {
                            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[i];
                            if (player.isInsideFactory)
                            {
                                Functions.SpawnMultipleEnemies(Variables.presetEnemiesToSpawn);
                                Variables.presetEnemiesToSpawn.Clear();
                            }
                        }
                    }

                    //Ship Landed and we do this once
                    if (StartOfRound.Instance.shipHasLanded && !Variables.Landed)
                    {
                        Variables.Landed = true;

                        //Spawn Enemies defined
                        if (Variables.presetEnemiesToSpawn.Count > 0)
                        {
                            if (!Variables.WaitUntilPlayerInside)
                            {
                                Functions.SpawnMultipleEnemies(Variables.presetEnemiesToSpawn);
                            }
                        }

                        //Start Inside Out Event
                        if (Variables.SpawnInsideOut)
                        {
                            Functions.InsideOutEnemies();
                        }
                    }

                    //Ship taking off set variables back
                    if (StartOfRound.Instance.shipIsLeaving)
                    {

                        //Ship Taking Off
                        if (!Variables.TakeOffExecuted)
                        {
                            //Give Free Money
                            if (Plugin.EnableFreeMoney.Value)
                            {
                                if (!StartOfRound.Instance.allPlayersDead)
                                {
                                    Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                                    terminal.groupCredits += Plugin.FreeMoneyValue.Value;
                                    terminal.SyncGroupCreditsServerRpc(terminal.groupCredits, terminal.numberOfItemsInDropship);
                                    HUDManager.Instance.AddTextToChatOnServer("\"<size=10><color=green> You survived another day! Here's free cash c: </color></size>", -1);
                                }
                                else
                                {
                                    HUDManager.Instance.AddTextToChatOnServer("\"<size=10><color=red> That was Brutal! No free cash today :c </color></size>", -1);
                                }

                            }

                            if (Variables.TurretSpawned)
                            {
                                Variables.mls.LogWarning("Disabling Turret due to ship leaving");
                                Turret[] Turrets = UnityEngine.Object.FindObjectsOfType<Turret>();
                                foreach (var Turret in Turrets)
                                {
                                    Turret.ToggleTurretEnabled(false);
                                }
                                Variables.TurretSpawned = false;
                            }

                            Functions.CleanUpAllVariables();
                            Variables.TakeOffExecuted = true;

                        }

                    }
                    else
                    {
                        Variables.TakeOffExecuted = false;
                    }

                }
            }
            catch (Exception ex)
            {
                Variables.mls.LogError($"Error in Update: {ex.Message}");
                BcpLogger.Log($"Error in Update: {ex.Message}");
            }

        }

        public static void CleanupAllSpawns()
        {
            foreach (GameObject gameObject in Variables.objectsToCleanUp)
            {
                if (gameObject != null)
                {
                    gameObject.GetComponent<NetworkObject>().Despawn(true);
                }
            }
            Variables.objectsToCleanUp.Clear();
        }
    }
}
