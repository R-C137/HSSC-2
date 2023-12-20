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
 *                   - Moved gift counting and display to Utility script (C137)
 */
using CsUtils;
using CsUtils.Systems.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Utility : Singleton<Utility>
{
    /// <summary>
    /// Class handling the scene loading animation
    /// </summary>
    public SceneLoader sceneLoader;

    /// <summary>
    /// The shower for the gift counter
    /// </summary>
    public TextMeshProUGUI giftCounterShower;

    /// <summary>
    /// The canvas for the game over UI
    /// </summary>
    public GameObject gameOverCanvas;

    /// <summary>
    /// Reference the to player model of the grinch
    /// </summary>
    public GameObject grinch;

    /// <summary>
    /// The amount of gifts the player has
    /// </summary>
    public int giftCounter;

    /// <summary>
    /// Raised when a gift is delivered
    /// </summary>
    /// <param name="giftCounter">The current value of the gift counter</param>
    public delegate void GiftDelivery(ref int giftCounter);
    public event GiftDelivery onGiftDelivered;

    private void Update()
    {
        giftCounterShower.text = $"Gifts: {giftCounter}";
    }

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
    /// Called when a gift is delivered
    /// </summary>
    public void GiftDelivered()
    {
        onGiftDelivered?.Invoke(ref giftCounter);
    }

    /// <summary>
    /// Restarts the game scene
    /// </summary>
    public void Restart()
    {
        sceneLoader.LoadScene(1);
    }
}
