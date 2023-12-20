/* Sled.cs - HSSC-2
 * 
 * Creation Date: 18/12/2023
 * Authors: Archetype
 * Original: Archetype
 * 
 * Edited By: Archetype
 * 
 * Changes: 
 *      [18/12/2023] - Initial implementation (Archetype)
 */
using UnityEngine;

public class Sled : MonoBehaviour
{
    /// <summary>
    /// The point to always lerp towards
    /// </summary>
    public Transform targetToMoveTowards;

    /// <summary>
    /// The point to always point at
    /// </summary>
    public Transform Reindeer;

    /// <summary>
    /// Lerp speed
    /// </summary>
    public float moveSpeed = 5f;

    void FixedUpdate()
    {
        // Move towards the target using Lerp
        if (targetToMoveTowards != null)
        {
            transform.position = Vector3.Lerp(transform.position, targetToMoveTowards.position, moveSpeed * Time.deltaTime);
        }

        // Look at the sleigh
        if (Reindeer != null)
        {
            transform.LookAt(Reindeer);
        }
    }
}
