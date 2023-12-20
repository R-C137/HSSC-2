/* GrinchBehaviour.cs - HSSC-2
 * 
 * Creation Date: 19/12/2023
 * Authors: C137, Archetype
 * Original: C137
 * 
 * Edited By: C137, Archetype
 * 
 * Changes: 
 *      [19/12/2023] - Initial implementation (C137)
 *                   - Spawning of gifts can now be disabled (C137)
 *                   - Made class a singleton (C137)
 *                   - Added behaviour for taking damage and retreating (Archetype)
 *     
 *      [20/12/2023] - Code review (C137)
 *                   - Improved grinch retreat animation (C137)
 *                   - Trap and gift spawning are now separate (C137)
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
    /// The different traps to be spawned
    /// </summary>
    public GameObject[] traps;

    /// <summary>
    /// The class handling the following of the player
    /// </summary>
    public AxisFollow followScript;

    /// <summary>
    /// The region in which to spawn the gifts
    /// </summary>
    public BoxCollider spawnRegion;

    /// <summary>
    /// The position at which the Grinch retreats to when hit
    /// </summary>
    public Transform retreatPosition;

    /// <summary>
    /// The interval at which to spawn the gifts
    /// </summary>
    public float spawnInterval = 2f;

    /// <summary>
    /// Whether the traps/gifts can be spawned
    /// </summary>
    public bool spawnObjects = true;

    /// <summary>
    /// How many hits needed for grinch to retreat
    /// </summary>
    public int retreatHits = 3;

    /// <summary>
    /// How long it takes before the grinch returns
    /// </summary>
    public float recoverTime = 5f;

    /// <summary>
    /// How long it takes for the Grinch to reach the retreat position and back
    /// </summary>
    public float retreatTime = 3f;

    /// <summary>
    /// The delay before the Grinch starts retreating after being hit enough times
    /// </summary>
    public float retreatDelay = 1.5f;

    /// <summary>
    /// The amount of lives the grinch starts with
    /// </summary>
    int startingLives;

    /// <summary>
    /// The default parent of the grinch model
    /// </summary>
    Transform defaultParent;

    private void Start()
    {
        startingLives = retreatHits;
        StartCoroutine(SpawnThings());
    }

    IEnumerator SpawnThings()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (!spawnObjects)
                yield return new WaitUntil(() => spawnObjects);

            GameObject gift = Instantiate(Random.Range(0, 2) == 1 ? gifts[Random.Range(0, gifts.Length)] : traps[Random.Range(0, traps.Length)]);

            gift.transform.position = spawnRegion.GetRandomPoint();

            gift.transform.parent = MapGeneration.singleton.spawnedTerrains[^1].transform;
        }
    }

    /// <summary>
    /// Called when the grinch is hit by a trap
    /// </summary> 
    [ContextMenu("Stimulate Hit")]
    public void GrinchHit()
    {
        retreatHits--;

        if (retreatHits != 0)
            return; 

        for (int i = 0; i < 8; i++)
        {
            GameObject gift = Instantiate(gifts[Random.Range(0, gifts.Length)]);

            gift.transform.position = spawnRegion.GetRandomPoint();

            gift.transform.parent = MapGeneration.singleton.spawnedTerrains[^1].transform;
        }

        spawnObjects = false;

        defaultParent = followScript.transform.parent;
        followScript.transform.SetParent(retreatPosition, true);

        LeanTween.moveLocal(followScript.gameObject, Vector3.zero, retreatTime)
                 .setDelay(retreatDelay)
                 .setOnStart(() => followScript.enabled = false)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() =>
                 {
                     followScript.transform.parent = defaultParent;
                     StartCoroutine(Recover());
                 });
    }

    IEnumerator Recover()
    {
        yield return new WaitForSeconds(recoverTime);

        followScript.enabled = true;
        spawnObjects = true;

        retreatHits = startingLives;


    }
}
