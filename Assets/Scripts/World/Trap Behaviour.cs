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
 */
using System.Collections;
using UnityEngine;

public class TrapBehaviour : MonoBehaviour
{
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
        if (shot)
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
        Destroy(gameObject);
    }

    IEnumerator ActivationTimer()
    {
        yield return new WaitForSeconds(activationTime);

        isActivated = true;

        onTrapActivated?.Invoke();
    }

    /// <summary>
    /// Shoots the trap back at the Grinch
    /// </summary>
    protected virtual void ShootBack()
    {
        transform.parent = Utility.singleton.grinch.transform;

        LeanTween.moveLocal(gameObject, Vector3.zero, shootBackTime).setEase(LeanTweenType.easeInElastic).setDestroyOnComplete(true);
    }
}
