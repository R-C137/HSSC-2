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
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// The class handling the loading of the scene
    /// </summary>
    public SceneLoader sceneLoader;

    /// <summary>
    /// Called when the play button is pressed
    /// </summary>
    public void Play()
    {
        sceneLoader.LoadScene(1);
    }

    /// <summary>
    /// Called when the settings button is pressed
    /// </summary>
    public void Settings()
    {

    }

    /// <summary>
    /// Called when the exit button is pressed
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}
