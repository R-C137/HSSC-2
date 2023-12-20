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
 *                   - Added behaviour for taking damage and retreating (Archetype)
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

    /// <summary>
    /// How many hits needed for grinch to retreat
    /// </summary>
    public int lives = 3;
    int startingLives;

    /// <summary>
    /// The script the grinch uses to follow the players position
    /// </summary>
    public AxisFollow followScript;

    /// <summary>
    /// The two targets for the followScript to alternate between
    /// </summary>
    public Transform reindeer, escapePos;

    /// <summary>
    /// How lonf it takes before the grinch returns
    /// </summary>
    public float recoverTime;

    private void Start()
    {
        startingLives = lives;
        StartCoroutine(SpawnThings());
    }

    IEnumerator SpawnThings()
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

    public void GetShot()
    {
        lives--;
        if (lives == 0)
        {
            for (int i = 0; i < 8; i++)
            {
                GameObject gift = Instantiate(gifts[0]);

                gift.transform.position = spawnRegion.GetRandomPoint();

                gift.transform.parent = MapGeneration.singleton.spawnedTerrains[^1].transform;
            }

            followScript.target = escapePos;
            StartCoroutine(Recover());
        }
    }

    IEnumerator Recover()
    {
        yield return new WaitForSeconds(recoverTime);
        followScript.target = reindeer;
        lives = startingLives;
    }
}
