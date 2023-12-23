/* SFXVolume.cs - HSSC-2
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

public class SFXVolume : MonoBehaviour
{
    /// <summary>
    /// The audio source whose volume will be adjusted
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// The default volume of the SFX
    /// </summary>
    public float defaultVolume;

    private void Start()
    {
        defaultVolume = audioSource.volume;

        SetVolume();

        Utility.singleton.onSettingsUpdated += SetVolume;
    }

    public void SetVolume()
    {
        audioSource.volume = defaultVolume * PlayerPrefs.GetFloat("settings.volume.sfx", 1) * PlayerPrefs.GetFloat("settings.volume.general", 1);
    }

    private void OnDestroy()
    {
        try
        {
            Utility.singleton.onSettingsUpdated -= SetVolume;
        }
        catch (System.Exception)
        {
            //Will only throw in the editor
        }
    }

    private void Reset()
    {
        TryGetComponent(out audioSource);
    }
}
