/* Utility.cs - HSSC-2
 * 
 * Creation Date: 19/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [19/12/2023] - Initial implementation (C137)
 */
using CsUtils;
using UnityEngine;

public class Utility : Singleton<Utility>
{
    /// <summary>
    /// Class handling the scene loading animation
    /// </summary>
    public SceneLoader sceneLoader;

    /// <summary>
    /// The canvas for the game over UI
    /// </summary>
    public GameObject gameOverCanvas;

    /// <summary>
    /// Reference the to player model of the grinch
    /// </summary>
    public GameObject grinch;

    /// <summary>
    /// Called when the game over menu needs to be shown
    /// </summary>
    public void DoGameOver()
    {
        //#TODO: Replace all this calls by raising an event on game over
        SantaBehaviour.singleton.shooting.ResetTimescale();
        SantaBehaviour.singleton.shooting.canMoveCamera = false;
        SantaBehaviour.singleton.shooting.canShoot = false;
        SantaBehaviour.singleton.shooting.shootingCamera.Priority = 1;
        GrinchBehaviour.singleton.spawnGifts = false;
        PlayerMovement.singleton.doMovement = false;

        gameOverCanvas.SetActive(true);
    }

    /// <summary>
    /// Restarts the game scene
    /// </summary>
    public void Restart()
    {
        sceneLoader.LoadScene(1);
    }
}
