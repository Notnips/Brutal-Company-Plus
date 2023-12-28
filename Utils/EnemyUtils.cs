using System.Collections.Generic;
using BrutalCompanyPlus.Objects;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyPlus.Utils;

internal static class EnemyUtils {
    private static readonly Dictionary<string, EnemyType> InsideEnemyTypes = new();
    private static readonly Dictionary<string, EnemyType> OutsideEnemyTypes = new();

    internal static void SpawnInsideEnemy(RoundManager Instance, GameObject EnemyPrefab) {
        // Pick a random vent to spawn from.
        if (!Instance.allEnemyVents.Random(out var vent)) return;
        // Retrieve the spawn position from the vent.
        var spawnPosition = vent.floorNode.position;
        var spawnRotation = Quaternion.Euler(0, vent.floorNode.eulerAngles.y, 0);
        // Spawn the enemy.
        var enemyObj = Object.Instantiate(EnemyPrefab, spawnPosition, spawnRotation);
        enemyObj.GetComponentInChildren<NetworkObject>().Spawn(true);
        // Retrieve the enemy AI component.
        var enemyAi = enemyObj.GetComponent<EnemyAI>();
        // If we're forcing an outside enemy to spawn inside, make sure we sync that to the clients.
        if (enemyAi.enemyType.isOutsideEnemy) {
            enemyAi.enemyType = SetOutsideEnemy(enemyAi.enemyType, false);
            BCNetworkManager.Instance.SyncEnemyTypeClientRpc(enemyAi, false);
        }

        // Add the enemy to the list of spawned enemies.
        Instance.SpawnedEnemies.Add(enemyAi);
    }

    internal static void SpawnOutsideEnemy(RoundManager Instance, GameObject EnemyPrefab) {
        // Pick a random node to spawn from.
        if (!GameObject.FindGameObjectsWithTag("OutsideAINode").Random(out var node)) return;
        // Spawn the enemy.
        var enemyObj = Object.Instantiate(EnemyPrefab, node.transform.position, Quaternion.identity);
        enemyObj.GetComponentInChildren<NetworkObject>().Spawn(true);
        // Retrieve the enemy AI component.
        var enemyAi = enemyObj.GetComponent<EnemyAI>();
        // If we're forcing an inside enemy to spawn outside, make sure we sync that to the clients.
        if (!enemyAi.enemyType.isOutsideEnemy) {
            enemyAi.enemyType = SetOutsideEnemy(enemyAi.enemyType, true);
            BCNetworkManager.Instance.SyncEnemyTypeClientRpc(enemyAi, true);
        }

        // Add the enemy to the list of spawned enemies.
        Instance.SpawnedEnemies.Add(enemyAi);
    }

    internal static EnemyType SetOutsideEnemy(EnemyType OriginalType, bool IsOutside) {
        switch (IsOutside) {
            case true when OutsideEnemyTypes.TryGetValue(OriginalType.enemyName, out var outsideEnemy):
                return outsideEnemy;
            case false when InsideEnemyTypes.TryGetValue(OriginalType.enemyName, out var insideEnemy):
                return insideEnemy;
        }

        var enemyType = Object.Instantiate(OriginalType);
        enemyType.isOutsideEnemy = IsOutside;

        if (IsOutside) OutsideEnemyTypes.Add(OriginalType.enemyName, enemyType);
        else InsideEnemyTypes.Add(OriginalType.enemyName, enemyType);

        return enemyType;
    }
}