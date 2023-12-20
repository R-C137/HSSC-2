/* entropy.cs - HSSC-2
 * 
 * Creation Date: 20/12/2023
 * Authors: Archetype
 * Original: Archetype
 * 
 * Edited By: Archetype
 * 
 * Changes: 
 *      [20/12/2023] - Initial implementation (Archetype)
 */
using UnityEngine;

public class entropy : MonoBehaviour
{
    /// <summary>
    /// How long it will take for this object to destroy itself
    /// </summary>
    public float lifespan = 5f;

    void Start()
    {
        Invoke("DestroySelf", lifespan);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
