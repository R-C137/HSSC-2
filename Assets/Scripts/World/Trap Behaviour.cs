/* TrapBehaviour.cs - HSSC-2
 * 
 * Creation Date: 19/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [19/12/2023] - Initial implementation (C137)
 *                   - Trap is now destroyed when it hits the grinch (C137)
 *                   - Made functions overridable (C137)
 *                   - Custom behaviours when the player hits the trap (C137)
 *      
 *      [20/12/2023] - Traps are now thrown by only moving their X position (C137)
 *                   - Grinch now gets affected by thrown back traps (C137)
 *                   
 *      [22/12/2023] - Added SFX support (C137)
 */
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TrapBehaviour : MonoBehaviour
{
    /// <summary>
    /// The audio source handling the playing of the SFXs
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// The different SFXs to play when the trap is hit by santa
    /// </summary>
    public AudioClip[] hitSFX;

    /// <summary>
    /// How long it takes for the trap to be shot back at the grinch
    /// </summary>
    public float shootBackTime = 3f;

    /// <summary>
    /// The time it takes for the gift to be activated
    /// </summary>
    public float activationTime = 3f;

    /// <summary>
    /// Whether the trap has been activated
    /// </summary>
    public bool isActivated;

    /// <summary>
    /// Whether the trap has been shot
    /// </summary>
    public bool shot;

    /// <summary>
    /// Event raised when the trap is activated
    /// </summary>
    public delegate void TrapActivated();
    public event TrapActivated onTrapActivated;

    public virtual void Start()
    {
        StartCoroutine(ActivationTimer());
    }

    /// <summary>
    /// Called when the trap is shot by the player
    /// </summary>
    public virtual void TrapShot()
    {
        if (shot || GrinchBehaviour.singleton.isRetreating)
            return;

        shot = true;

        if(isActivated)
        {
            Destroy(gameObject);
            return;
        }

        ShootBack();
    }

    /// <summary>
    /// Called when the player physically hits this trap
    /// </summary>
    public virtual void TrapHit()
    {
        if (shot)
            return;

        audioSource.transform.parent = null;
        audioSource.clip = hitSFX[Random.Range(0, hitSFX.Length)];
        audioSource.Play();

        Destroy(gameObject);
    }

    public virtual void Activate()
    {
        isActivated = true;

        onTrapActivated?.Invoke();

        PlaySFX();
    }

    public virtual void PlaySFX()
    {
        audioSource.clip = Utility.singleton.commonSFX.trapActivation[Random.Range(0, Utility.singleton.commonSFX.trapActivation.Length)];
        audioSource.Play();
    }

    IEnumerator ActivationTimer()
    {
        yield return new WaitForSeconds(activationTime);

        if (isActivated)
            yield break;

        Activate();
    }

    /// <summary>
    /// Shoots the trap back at the Grinch
    /// </summary>
    protected virtual void ShootBack()
    {
        transform.parent = Utility.singleton.grinch.transform;

        LeanTween.moveLocalX(gameObject, 0, shootBackTime).setEase(LeanTweenType.easeInElastic).setOnComplete(() =>
        {
            if(!GrinchBehaviour.singleton.isRetreating)
                GrinchBehaviour.singleton.GrinchHit();
            Destroy(gameObject);
        });
    }

    private void Reset()
    {
        TryGetComponent(out audioSource);
    }
}