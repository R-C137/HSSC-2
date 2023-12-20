/* GrinchBehaviour.cs - HSSC-2
 * 
 * Creation Date: 19/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [19/12/2023] - Initial implementation (C137)
 *                   - Spawning of gifts can now be disabled (C137)
 *                   - Made class a singleton (C137)
 */
using CsUtils;
using CsUtils.Extensions;
using System.Collections;
using UnityEngine;

public class GrinchBehaviour : Singleton<GrinchBehaviour>
{
    /// <summary>
    /// The different gifts to be spawned
    /// </summary>
    public GameObject[] gifts;

    /// <summary>
    /// The region in which to spawn the gifts
    /// </summary>
    public BoxCollider spawnRegion;

    /// <summary>
    /// The interval at which to spawn the gifts
    /// </summary>
    public float spawnInterval = 2f;

    /// <summary>
    /// Whether gifts can be spawned
    /// </summary>
    public bool spawnGifts = true;

    private void Start()
    {
        StartCoroutine(SpawnGifts());
    }

    IEnumerator SpawnGifts()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (!spawnGifts)
                yield return new WaitUntil(() => spawnGifts);

            GameObject gift = Instantiate(gifts[Random.Range(0, gifts.Length)]);

            gift.transform.position = spawnRegion.GetRandomPoint();

            gift.transform.parent = MapGeneration.singleton.spawnedTerrains[^1].transform;
        }
    }
}
