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

    public override void Start()
    {
        base.Start();

        onTrapActivated += BombActivated;
    }

    public override void TrapHit()
    {
        if (isActivated)
        {
            SantaBehaviour.singleton.lives--;

            Logging.singleton.Log("Santa has hit an activated bomb and has been damaged", LogSeverity.Info);
        }
        Destroy(gameObject);
    }

    private void BombActivated()
    {
        LeanTween.scale(gameObject, transform.localScale * 2, explosionDelay).setOnComplete(() =>
        {
            Collider[] colliders = new Collider[1];

            int hitCount = Physics.OverlapBoxNonAlloc(effectRegion.transform.position + effectRegion.center, effectRegion.size / 2, colliders, Quaternion.identity, mask);

            if (hitCount > 0)
            {
                SantaBehaviour.singleton.lives--;
                Logging.singleton.Log("Santa was hit by an explosion from a bomb", LogSeverity.Info);
            }
            Destroy(gameObject);
        });
    }

}
