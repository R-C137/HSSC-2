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
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]

public class UISFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    /// <summary>
    /// The audio source used to play the SFX
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// The SFX to be played on hover
    /// </summary>
    public AudioClip hoverSFX;

    /// <summary>
    /// The SFX to be played on click
    /// </summary>
    public AudioClip clickSFX;

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.clip = hoverSFX;
        audioSource.Play();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        audioSource.clip = clickSFX;
        audioSource.Play();
    }

    private void Reset()
    {
        TryGetComponent(out audioSource);
    }
}
