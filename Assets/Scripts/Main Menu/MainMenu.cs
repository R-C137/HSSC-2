/* MainMenu.cs - HSSC-2
 * 
 * Creation Date: 18/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [18/12/2023] - Initial implementation (C137)
 *      [21/12/2023] - Added exit button functionality (C137)
 *      [22/12/2023] - Removed settings handling (C137)
 *      [23/12/2023] - Added metrics support (C137)
 */
using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct Metrics
{
    /// <summary>
    /// The game object handling all of the metrics
    /// </summary>
    public GameObject metrics;

    /// <summary>
    /// The maximum distance flown by the player in a single run
    /// </summary>
    public TextMeshProUGUI maxDistance;

    /// <summary>
    /// The maximum amount of gifts delivered by the player in a single run
    /// </summary>
    public TextMeshProUGUI giftsDelivered;

    /// <summary>
    /// The total time played by the player
    /// </summary>
    public TextMeshProUGUI timePlayed;

    /// <summary>
    /// The maximum combo achieved by the player in a single run
    /// </summary>
    public TextMeshProUGUI maxCombo;
}

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Contains all of the metrics tracked
    /// </summary>
    public Metrics metrics;
    
    /// <summary>
    /// The class handling the cutscene
    /// </summary>
    public CutsceneHandler cutscene;

    /// <summary>
    /// The notice for the webGL build
    /// </summary>
    public TextMeshProUGUI webGLNotice;

    private void Start()
    {
        if (PlayerPrefs.GetInt("firstTimePlay", 1) == 0)
        {
            metrics.maxDistance.text = $"Distance Flown: {Mathf.Round(PlayerPrefs.GetFloat("metrics.maxDistance")):N0} M";
            metrics.giftsDelivered.text = $"Gifts Delivered: {PlayerPrefs.GetInt("metrics.giftsDelivered"):N0}";
            metrics.timePlayed.text = $"Time Played: {Mathf.Round(PlayerPrefs.GetInt("metrics.timePlayed")):N0} Minutes";
            metrics.maxCombo.text = $"Maximum Combo: X{PlayerPrefs.GetInt("metrics.maxCombo")}";
        }
        else
            metrics.metrics.SetActive(false);

        if (Application.platform == RuntimePlatform.WebGLPlayer)
            webGLNotice.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called when the play button is pressed
    /// </summary>
    public void Play()
    {
        cutscene.DoCutscene();
    }

    /// <summary>
    /// Called when the exit button is pressed
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}
