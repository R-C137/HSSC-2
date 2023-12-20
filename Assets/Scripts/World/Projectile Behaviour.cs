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
        if (other.CompareTag("Gift"))
        {
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