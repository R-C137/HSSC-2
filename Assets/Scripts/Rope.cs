/* Rope.cs - HSSC-2
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

public class Rope : MonoBehaviour
{
    /// <summary>
    /// Points that connect the rope
    /// </summary>
    public Transform startPoint, endPoint;

    /// <summary>
    /// Rope thickness
    /// </summary>
    public float radius = 0.5f;  

    private void Update()
    {
        if (startPoint != null && endPoint != null)
        {
            // Calculate the direction vector between the two points
            Vector3 direction = endPoint.position - startPoint.position;

            // Set the position to the midpoint between the two points
            transform.position = (startPoint.position + endPoint.position) / 2f;

            // Set the scale to stretch the cylinder between the two points
            transform.localScale = new Vector3(radius * 2f, direction.magnitude / 2f, radius * 2f);

            // Rotate the cylinder to align with the direction vector
            transform.up = direction.normalized;
        }
    }
}
