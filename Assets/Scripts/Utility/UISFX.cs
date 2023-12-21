/* UISFX.cs - HSSC-2
 * 
 * Creation Date: 20/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [20/12/2023] - Initial implementation (C137)
 *      [21/12/2023] - Added support for new SFX handling system (C137)
 *                   - Removed unnecessary using statements (C137)
 */
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]

public class UISFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    /// <summary>
    /// The audio source used to play the SFX
    /// </summary>
    public AudioSource audioSource;

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.clip = Utility.singleton.commonSFX.UIHover;
        audioSource.Play();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        audioSource.clip = Utility.singleton.commonSFX.UIClick;
        audioSource.Play();
    }

    private void Reset()
    {
        TryGetComponent(out audioSource);
    }
}
