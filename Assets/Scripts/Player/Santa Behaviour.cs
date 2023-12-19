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
 *                   - Made script a singleton (C137)
 *                   - Custom trap hitting (C137)
 */
using CsUtils;
using CsUtils.Systems.Logging;
using UnityEngine;

public class SantaBehaviour : Singleton<SantaBehaviour>
{
    /// <summary>
    /// Reference to the shooting handler
    /// </summary>
    public Shooting shooting;

    /// <summary>
    /// The number of lives the player has
    /// </summary>
    public int lives
    {
        get
        {
            return _lives;
        }
        set
        {
            _lives = value;
            LifeUpdated();
        }
    }

    [SerializeField, InspectorName("Lives")]
    private int _lives;

    /// <summary>
    /// Called when the player's lives are updated
    /// </summary>
    void LifeUpdated()
    {
        if(lives <= 0)
            Utility.singleton.DoGameOver();
    }

    [ContextMenu("Reduce Life")]
    void ReduceLife()
    {
        lives--;
        Logging.singleton.Log("Life is now at {0}", LogSeverity.Info, parameters: lives);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gift"))
        {
            shooting.giftCounter++;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Trap"))
        {
            other.GetComponent<TrapBehaviour>().TrapHit();
        }
    }
}
