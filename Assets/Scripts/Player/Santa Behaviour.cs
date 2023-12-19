/* SantaBehaviour.cs - HSSC-2
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
using CsUtils.Systems.Logging;
using UnityEngine;

public class SantaBehaviour : MonoBehaviour
{
    /// <summary>
    /// The number of lives the player has
    /// </summary>
    public int lives = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gift"))
        {
            // Increase gift counter
        }
        if (other.CompareTag("Trap"))
        {
            lives--;
            if (lives <= 0)
                Logging.singleton.Log("Game over", LogSeverity.Info);
        }
    }
}
