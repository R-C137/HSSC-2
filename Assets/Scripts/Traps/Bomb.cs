/* Bomb.cs - HSSC-2
 * 
 * Creation Date: 19/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [19/12/2023] - Initial implementation (C137)
 *                   - Improved hit behaviour (C137)
 *                   
 *      [23/12/2023] - Added explosion SFX support (C137)
 *                   - Improved explosion zone calculation (C137)
 */
using CsUtils.Systems.Logging;
using UnityEngine;

public class Bomb : TrapBehaviour
{
    /// <summary>
    /// The delay before the explosion occurs
    /// </summary>
    public float explosionDelay;

    /// <summary>
    /// The region in which the explosion affects
    /// </summary>
    public BoxCollider effectRegion;

    /// <summary>
    /// The layer mask to use for the box cast
    /// </summary>
    public LayerMask mask;

    /// <summary>
    /// The audio source handling the SFX of the bomb
    /// </summary>
    public AudioSource bombSFX;

    /// <summary>
    /// The different SFXs to play when the bomb explodes
    /// </summary>
    public AudioClip[] bombExplode;

    public override void Start()
    {
       // base.Start();

       // onTrapActivated += BombActivated;
    }

    public override void TrapHit()
    {
        if (isActivated)
        {
            SantaBehaviour.singleton.lives--;

            Logging.singleton.Log("Santa has hit an activated bomb and has been damaged", LogSeverity.Info);
        }

        base.TrapHit();
    }

    public override void Activate()
    {
        base.Activate();
        BombActivated();
    }

    private void BombActivated()
    {
        LeanTween.scale(gameObject, transform.localScale * 2, explosionDelay).setOnComplete(() =>
        {
            bombSFX.clip = bombExplode[Random.Range(0, bombExplode.Length)];
            bombSFX.Play();

            bombSFX.transform.parent = null;

            LeanTween.delayedCall(5f, () =>
            {
                if(bombSFX != null && bombSFX.gameObject != null)
                Destroy(bombSFX.gameObject);
            });

            Collider[] colliders = new Collider[1];

            int hitCount = Physics.OverlapBoxNonAlloc(effectRegion.transform.position + effectRegion.center, effectRegion.size / 2 * effectRegion.transform.lossyScale.x, colliders, Quaternion.identity, mask);

            if (hitCount > 0)
            {
                SantaBehaviour.singleton.lives--;
                Logging.singleton.Log("Santa was hit by an explosion from a bomb", LogSeverity.Info);
            }
            Destroy(gameObject);
        }).setDelay(.5f);
    }

}