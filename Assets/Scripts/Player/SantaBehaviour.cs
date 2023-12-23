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
 *                   - Added a debug for hitting the floor, to be replaced with game over menu or whatever other mechanic (Archetype)
 *                   
 *      [20/12/2023] - Natural obstacle handling (C137)
 *                   - Added behaviour for changing distance to the grinch and candy cane (speed boost) (Archetype)
 *                   
 *      [21/12/2023] - Code review (C137)
 *                   - Camera shake is now also done for obstacle collisions (C137)
 *                   - Gift counter now no longer goes below 0 (C137)
 *                   
 *      [22/12/2023] - Added SFX support (C137)
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
    /// The audio source handling the playing of the SFX
    /// </summary>
    public AudioSource sfxHandler;

    /// <summary>
    /// SFX to play when a natural obstacle is hit
    /// </summary>
    public AudioClip[] naturalObstacleHit;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gift"))
        {
            Utility.singleton.giftCounter++;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Trap"))
        {
            other.GetComponent<TrapBehaviour>().TrapHit();
            GrinchPositionalHandling.singleton.MoveFurther();

            Utility.singleton.ShakeCamera(5f, 1f);
        }
        if(other.CompareTag("Natural Obstacle"))
        {
            Utility.singleton.giftCounter = Mathf.Max(0, Utility.singleton.giftCounter - 1);
            Destroy(other.gameObject);
            GrinchPositionalHandling.singleton.MoveFurther();

            Utility.singleton.ShakeCamera(5f, 1f);

            //sfxHandler.clip = naturalObstacleHit[Random.Range(0, naturalObstacleHit.Length)];
            //sfxHandler.Play();
        }
        if (other.CompareTag("Floor"))
        {
            Debug.Log("Ahh i hit the floor :(");
            GrinchPositionalHandling.singleton.MoveFurther();
        }
        if (other.CompareTag("Candy Cane"))
        {
            GrinchPositionalHandling.singleton.MoveCloser();
            Destroy(other.gameObject);
        }
    }
}