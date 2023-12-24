/* SettingsMenu.cs - HSSC-2
 * 
 * Creation Date: 22/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [22/12/2023] - Game pausing support (C137)
 */
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    /// <summary>
    /// The slider for the general volume
    /// </summary>
    public Slider generalVolume;

    /// <summary>
    /// The slider for the music volume
    /// </summary>
    public Slider musicVolume;

    /// <summary>
    /// The slider for the SFX volume
    /// </summary>
    public Slider sfxVolume;

    /// <summary>
    /// The slider for the mouse sensitivity
    /// </summary>
    public Slider mouseSensitivity;

    private void Start()
    {
        SetSliderValues();
    }

    void SetSliderValues()
    {
        generalVolume.value = PlayerPrefs.GetFloat("settings.volume.general", 1);
        musicVolume.value = PlayerPrefs.GetFloat("settings.volume.music", 1);
        sfxVolume.value = PlayerPrefs.GetFloat("settings.volume.sfx", 1);
        mouseSensitivity.value = PlayerPrefs.GetFloat("settings.mouseSensitivity", .5f);
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        SetSliderValues();
    }

    public void GeneralVolumeSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("settings.volume.general", value);
    }

    public void MusicVolumeSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("settings.volume.music", value);
    }

    public void SFXVolumeSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("settings.volume.sfx", value);
    }

    public void MouseSensSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("settings.mouseSensitivity", value);
    }
}
