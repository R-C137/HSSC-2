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
 */
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.isTrigger)
            return;

        if (collision.collider.CompareTag("Gift"))
        {
            Destroy(collision.gameObject);
        }
        if (collision.collider.CompareTag("Trap"))
        {
            collision.collider.GetComponent<TrapBehaviour>().TrapShot();
        }
        if (collision.collider.CompareTag("Delivery"))
        {
            Utility.singleton.GiftDelivered();
        }

        Destroy(gameObject);
    }
}
