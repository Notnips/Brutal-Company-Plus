using BrutalCompanyPlus.Objects;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyPlus.Utils;

internal static class EnemyUtils {
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
        // We might be forcing an outside enemy to spawn inside, so ensure this is set to false.
        enemyAi.enemyType = SetOutsideEnemy(enemyAi.enemyType, false);
        BCNetworkManager.Instance.SyncEnemyTypeClientRpc(enemyAi, false);
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
        // We might be forcing an inside enemy to spawn outside, so ensure this is set to true.
        enemyAi.enemyType = SetOutsideEnemy(enemyAi.enemyType, true);
        BCNetworkManager.Instance.SyncEnemyTypeClientRpc(enemyAi, true);
        // Add the enemy to the list of spawned enemies.
        Instance.SpawnedEnemies.Add(enemyAi);
    }

    internal static EnemyType SetOutsideEnemy(EnemyType OriginalType, bool IsOutside) {
        var enemyType = Object.Instantiate(OriginalType);
        enemyType.isOutsideEnemy = IsOutside;
        return enemyType;
    }
}