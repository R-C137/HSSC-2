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
 *      [23/12/2023] - Added anvil collection support (C137)
 *                   - Added gift SFX support (C137)
 */
using CsUtils;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct LifeDisplay
{
    /// <summary>
    /// The sprite to use for a full heart
    /// </summary>
    public Sprite fullHeart;

    /// <summary>
    /// The sprite to use for an empty heart
    /// </summary>
    public Sprite emptyHeart;

    /// <summary>
    /// The images to display the hearts
    /// </summary>
    public Image[] hearts;
}

public class SantaBehaviour : Singleton<SantaBehaviour>
{
    /// <summary>
    /// Reference to the shooting handler
    /// </summary>
    public Shooting shooting;

    /// <summary>
    /// Contains information about the displaying of the player's lives in the UI
    /// </summary>
    public LifeDisplay lifeDisplay;

    /// <summary>
    /// The audio source handling the playing of the SFX
    /// </summary>
    public AudioSource sfxHandler;

    /// <summary>
    /// The audio source handling the playing of the gift SFX
    /// </summary>
    public AudioSource giftSFX;

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
            LifeUpdated(value);
            _lives = value;
        }
    }

    [SerializeField, InspectorName("Lives")]
    private int _lives;

    /// <summary>
    /// How long it takes for a life to be restored after it has been removed
    /// </summary>
    public float lifeRestoreCooldown = 15f;


    /// <summary>
    /// The id of the tween handling the lives
    /// </summary>
    int lifeTween;

    /// <summary>
    /// Called when the player's lives are updated
    /// </summary>
    void LifeUpdated(int value)
    {
        if (value < _lives)
        {
            if (value <= 0)
                Utility.singleton.DoGameOver();

            Utility.singleton.ShakeCamera();
        }

        for (int i = 0; i < lifeDisplay.hearts.Length; i++)
        {
            lifeDisplay.hearts[i].sprite = value - 1 < i ? lifeDisplay.emptyHeart : lifeDisplay.fullHeart;
        }

        LeanTween.cancel(lifeTween, false);
        if(value != 3)
            lifeTween = LeanTween.delayedCall(lifeRestoreCooldown, () => lives++).uniqueId;

    }

    [ContextMenu("remove life")]
    void removelife()
    {
        lives--;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gift"))
        {
            Utility.singleton.giftCounter++;

            giftSFX.clip = Utility.singleton.commonSFX.giftCollected[Random.Range(0, Utility.singleton.commonSFX.giftCollected.Length)];
            giftSFX.Play();

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

            Utility.singleton.ShakeCamera();

            sfxHandler.clip = naturalObstacleHit[Random.Range(0, naturalObstacleHit.Length)];
            sfxHandler.Play();
        }
        if (other.CompareTag("Floor"))
        {
            Debug.Log("Ahh i hit the floor :(");
            GrinchPositionalHandling.singleton.MoveFurther();

            Utility.singleton.ShakeCamera();
        }
        if (other.CompareTag("Anvil"))
        {
            Shooting.singleton.shootingAnvil = true;
            Destroy(other.gameObject);
        }
    }

    private void OnDisable()
    {
        LeanTween.cancel(lifeTween);
    }
}