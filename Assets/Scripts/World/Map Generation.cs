/* MapGeneration.cs - HSSC-2
 * 
 * Creation Date: 18/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [18/12/2023] - Initial implementation (C137)
 *      
 *      [19/12/2023] - Renamed fields (C137)
 */
using CsUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : Singleton<MapGeneration>
{
    /// <summary>
    /// The target whom to spawn the terrain around
    /// </summary>
    public GameObject target;

    /// <summary>
    /// The different types of terrain to spawn
    /// </summary>
    public GameObject[] terrain;

    /// <summary>
    /// The distance of the target from the last terrain to spawn a new one
    /// </summary>
    public float spawnX = 100f;

    /// <summary>
    /// The distance of the target from the terrain at which it should be destroyed
    /// </summary>
    public float killX = 100f;

    /// <summary>
    /// Reference to all the terrains in the map
    /// </summary>
    [Header("Debug Utils")]
    public List<GameObject> spawnedTerrains = new();

    void Start()
    {
        // Spawn initial terrain
        SpawnTerrain(target.transform.position.x);
    }

    void Update()
    {
        // Spawn and destroy terrain as needed
        SpawnTerrain(target.transform.position.x + spawnX);
        DestroyTerrain(target.transform.position.x - killX);
    }

    void SpawnTerrain(float upToX)
    {
        float lastX = spawnedTerrains.Count > 0 ? spawnedTerrains[^1].transform.position.x : 0;

        while (lastX < upToX)
        {
            // Choose a random terrain prefab
            GameObject prefab = terrain[Random.Range(0, terrain.Length)];

            float terrainSize = prefab.GetComponent<Renderer>().bounds.size.x; /*is there a better way to get the x size of the prefab here ?*/

            // Calculate position for new terrain
            Vector3 position = new(lastX + terrainSize, prefab.transform.position.y, prefab.transform.position.z);

            // Instantiate new terrain and add it to the list
            GameObject newTerrain = Instantiate(prefab, position, Quaternion.identity);
            spawnedTerrains.Add(newTerrain);

            lastX += terrainSize;
        }
    }

    void DestroyTerrain(float upToX)
    {
        for (int i = 0; i >= 0; i--)
        {
            if (spawnedTerrains[i].transform.position.x < upToX)
            {
                // Destroy terrain and remove it from the list
                Destroy(spawnedTerrains[i]);
                spawnedTerrains.RemoveAt(i);
            }
            else
                break;
        }
    }
}
