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
 *                   - Added smoke particle effect when hit (Archetype)
 *                   
 *      [22/12/2023] - Game pausing support (C137)
 *      [23/12/2023] - Improved support for now gift spawning system (C137)
 */
using CsUtils;
using CsUtils.Extensions;
using System.Collections;
using UnityEngine;

[System.Serializable]
public struct GrinchSFXChanced
{
    /// <summary>
    /// The SFX to play
    /// </summary>
    public AudioClip SFX;

    /// <summary>
    /// The probability of this SFX to play
    /// </summary>
    public float probability;
}

[System.Serializable]
public struct GrinchSFX
{
    /// <summary>
    /// The SFXs to play when the grinch is hit
    /// </summary>
    public GrinchSFXChanced[] hit;

    /// <summary>
    /// The SFX to play when the grinch retreats
    /// </summary>
    public AudioClip retreat;
}

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
    /// The size multipliers for the gifts
    /// </summary>
    public float minGiftSizeMultiplier, maxGiftSizeMultiplier;

    /// <summary>
    /// The different SFXs to play
    /// </summary>
    public GrinchSFX sfx;

    /// <summary>
    /// The audio source handling the hit SFX
    /// </summary>
    public AudioSource hitSFX;

    /// <summary>
    /// The audio source handling the retreat SFX
    /// </summary>
    public AudioSource retreatSFX;

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
    /// Whether the grinch is currently retreating
    /// </summary>
    public bool isRetreating;

    /// <summary>
    /// The amount of lives the grinch starts with
    /// </summary>
    int startingLives;

    /// <summary>
    /// The default parent of the grinch model
    /// </summary>
    Transform defaultParent;

    /// <summary>
    /// Smoke particle effect prefab
    /// </summary>
    public GameObject smokePoof;

    private void Start()
    {
        Utility.singleton.onGamePaused += HandlePausing;

        startingLives = retreatHits;
        StartCoroutine(SpawnThings());
    }

    private void HandlePausing(bool doPausing)
    {
        spawnObjects = !doPausing;
    }

    IEnumerator SpawnThings()
    {
        while (true)
        {
            yield return new WaitForSeconds(GrinchPositionalHandling.singleton.positions[GrinchPositionalHandling.singleton.currentPosition].objectSpawnInterval);

            if (!spawnObjects)
                yield return new WaitUntil(() => spawnObjects);

            GameObject gift = Instantiate(gifts[Random.Range(0, gifts.Length)]);
            if (Random.Range(0, 2) == 1)
            {
                var trapHandler = gift.GetComponent<SurpriseInside>();
                GameObject trap = traps[Random.Range(0, traps.Length)];

                trapHandler.trap = trap;
                trapHandler.TrapInside(trap.GetComponent<TrapBehaviour>().activationTime);
            }

            gift.transform.position = spawnRegion.GetRandomPoint();
            gift.transform.localScale *= Random.Range(minGiftSizeMultiplier, maxGiftSizeMultiplier);
            gift.transform.parent = MapGeneration.singleton.spawnedTerrains[^1].transform;
        }
    }

    /// <summary>
    /// Called when the grinch is hit by a trap
    /// </summary> 
    [ContextMenu("Stimulate Hit")]
    public void GrinchHit()
    {
        Debug.Log("Hit");
        retreatHits--;

        Instantiate(smokePoof, followScript.gameObject.transform);

        PlayHitSFX();

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

        PlayRetreatSFX();

        TrashTalkHandler.singleton.audioSource.Stop();

        void PlayHitSFX()
        {
            WeightedNumber[] weights = new WeightedNumber[sfx.hit.Length];
            for (int i = 0; i < sfx.hit.Length; i++)
            {
                weights[i] = new WeightedNumber()
                {
                    probability = sfx.hit[i].probability,
                    number = i
                };
            }

            hitSFX.clip = sfx.hit[StaticUtils.WeightedRandom(weights)].SFX;
            hitSFX.Play();
        }

        void PlayRetreatSFX()
        {
            retreatSFX.clip = sfx.retreat;
            retreatSFX.Play();
        }
    }

    IEnumerator Recover()
    {
        yield return new WaitForSeconds(recoverTime);

        followScript.enabled = true;
        spawnObjects = true;

        retreatHits = startingLives;
    }
}