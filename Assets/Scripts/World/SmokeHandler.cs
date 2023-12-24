/* SmokeHandler.cs - HSSC-2
 * 
 * Creation Date: 24/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [24/12/2023] - Game pausing support (C137)
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeHandler : MonoBehaviour
{
    /// <summary>
    /// The distance the player needs to be before the delivery points are shown
    /// </summary>
    public float spawnDistance;

    private void FixedUpdate()
    {
        foreach (Transform child in transform)
        {
            if (Vector3.Distance(child.position, PlayerMovement.singleton.player.transform.position) < spawnDistance)
                child.gameObject.SetActive(true);
        }
    }
}
