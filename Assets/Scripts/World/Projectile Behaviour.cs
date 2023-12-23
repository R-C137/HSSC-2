/* ProjectileBehaviour.cs - HSSC-2
 * 
 * Creation Date: 19/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [19/12/2023] - Initial implementation (C137)
 *                   - Changed interactions to Triggers to avoid possible physics issues and added grinch damage (Archetype)
 *                   - Added lifespan to avoid letting them go on forever (Archetype)
 */
using System.Collections;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    /// <summary>
    /// How long it takes for the projectile to disappear
    /// </summary>
    public float lifeSpan = 10;

    /// <summary>
    /// Whether the current projectile is an anvil
    /// </summary>
    public bool isAnvil;

    private void Start()
    {
        StartCoroutine(Entropy());
    }

    IEnumerator Entropy()
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
      {
        if (isAnvil)
        {
            if (other.CompareTag("Grinch"))
            {
                GrinchPositionalHandling.singleton.MoveCloser();
            }

            return;
        }

        if (other.CompareTag("Gift"))
        {
            if (other.gameObject.TryGetComponent(out SurpriseInside surprise))
            {
                if (surprise.isTrap)
                    surprise.ForceReplacement();
                else
                    Destroy(other.gameObject);
            }
            else
                Destroy(other.gameObject);
            Destroy(gameObject);
        }
        if (other.CompareTag("Trap"))
        {
            other.GetComponent<TrapBehaviour>().TrapShot();
            Destroy(gameObject);
        }
        if (other.CompareTag("Delivery"))
        {
            Utility.singleton.GiftDelivered();
            Destroy(gameObject);
        }
    }
}