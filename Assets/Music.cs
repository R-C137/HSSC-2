/* DeliveryHandler.cs - HSSC-2
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
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource audioSource;

    private void Start()
    {
        Utility.singleton.onSettingsUpdated += UpdateMusicVolume;

        UpdateMusicVolume();
    }

    public void UpdateMusicVolume()
    {
        audioSource.volume = PlayerPrefs.GetFloat("settings.volume.general", 1) * PlayerPrefs.GetFloat("settings.volume.music", 1);
    }

    private void Reset()
    {
        TryGetComponent(out audioSource);
    }
}
