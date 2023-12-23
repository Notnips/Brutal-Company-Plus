using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyPlus.Utils;

internal static class EnemyUtils {
    internal static void SpawnInsideEnemy(RoundManager Instance, GameObject EnemyPrefab) {
        // Pick a random vent to spawn from.
        var vent = Instance.allEnemyVents.Random();
        // Retrieve the spawn position from the vent.
        var spawnPosition = vent.floorNode.position;
        var spawnRotation = Quaternion.Euler(0, vent.floorNode.eulerAngles.y, 0);
        // Spawn the enemy.
        var enemyObj = Object.Instantiate(EnemyPrefab, spawnPosition, spawnRotation);
        enemyObj.GetComponentInChildren<NetworkObject>().Spawn(true);
        // Retrieve the enemy AI component.
        var enemyAi = enemyObj.GetComponent<EnemyAI>();
        // We might be forcing an outside enemy to spawn inside, so ensure this is set to false.
        enemyAi.enemyType.isOutsideEnemy = false;
        // Add the enemy to the list of spawned enemies.
        Instance.SpawnedEnemies.Add(enemyAi);
    }

    internal static void SpawnOutsideEnemy(RoundManager Instance, GameObject EnemyPrefab) {
        // Pick a random node to spawn from.
        var node = GameObject.FindGameObjectsWithTag("OutsideAINode").Random();
        // Spawn the enemy.
        var enemyObj = Object.Instantiate(EnemyPrefab, node.transform.position, Quaternion.identity);
        enemyObj.GetComponentInChildren<NetworkObject>().Spawn(true);
        // Retrieve the enemy AI component.
        var enemyAi = enemyObj.GetComponent<EnemyAI>();
        // We might be forcing an inside enemy to spawn outside, so ensure this is set to true.
        enemyAi.enemyType.isOutsideEnemy = true;
        // Add the enemy to the list of spawned enemies.
        Instance.SpawnedEnemies.Add(enemyAi);
    }
}