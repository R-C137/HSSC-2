/* NaturalObstacles.cs - HSSC-2
 * 
 * Creation Date: 20/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [20/12/2023] - Initial implementation (C137)
 *
 *      [22/12/2023] - Game pausing support (C137)
 */
using CsUtils;
using CsUtils.Extensions;
using System.Collections;
using UnityEngine;

[System.Serializable]
public struct ObstacleInfo
{
    /// <summary>
    /// The prefab to spawn
    /// </summary>
    public GameObject prefab;

    /// <summary>
    /// How long it takes the obstacle to reach the other side
    /// </summary>
    public float moveSpeed;

    /// <summary>
    /// How likely is it that this obstacle will be spawned
    /// </summary>
    public float spawnChance;
}

public class NaturalObstacleSpawner: MonoBehaviour
{
    /// <summary>
    /// The different obstacles to spawn
    /// </summary>
    public ObstacleInfo[] obstacles;

    /// <summary>
    /// The right and the left spawn regions for the obstacles
    /// </summary>
    public BoxCollider[] spawnRegions = new BoxCollider[2];

    /// <summary>
    /// The delay in between spawning obstacles
    /// </summary>
    public float spawnDelay;

    /// <summary>
    /// Whether obstacles can be spawned
    /// </summary>
    public bool canSpawnObstacles = true;

    /// <summary>
    /// The coroutine used to spawn objects
    /// </summary>
    Coroutine spawnObjectsCoroutine;

    void Start()
    {
        spawnObjectsCoroutine = StartCoroutine(SpawnObjects());

        Utility.singleton.onGamePaused += HandlePausing;
    }

    private void HandlePausing(bool doPausing)
    {
        if (doPausing)
            StopCoroutine(spawnObjectsCoroutine);
        else
            spawnObjectsCoroutine = StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);

            if (!canSpawnObstacles)
                yield return new WaitUntil(() => canSpawnObstacles);

            ObstacleInfo obstacle = GetObstacle();

            GameObject obj = Instantiate(obstacle.prefab);
            obj.transform.parent = MapGeneration.singleton.spawnedTerrains[^1].transform;

            int spawnRegion = Random.Range(0, 2);
            obj.transform.position = spawnRegions[spawnRegion].GetRandomPoint();

            LeanTween.moveZ(obj, spawnRegions[spawnRegion == 0 ? 1 : 0].transform.position.z, obstacle.moveSpeed);
        }
    }

    ObstacleInfo GetObstacle()
    {
        WeightedNumber[] weights = new WeightedNumber[obstacles.Length];

        for (int i = 0; i < obstacles.Length; i++)
        {
            weights[i] = new WeightedNumber()
            {
                number = i,
                probability = obstacles[i].spawnChance
            };
        }

        return obstacles[StaticUtils.WeightedRandom(weights)];
    }
}
