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
using static UnityEngine.Rendering.DebugUI;

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
    /// How happy the world is
    /// </summary>
    public int happiness;

    /// <summary>
    /// The sprites used to display the different levels of happiness
    /// </summary>
    public Sprite[] happinessSprites;

    /// <summary>
    /// The shower used to display the happiness of thee world
    /// </summary>
    public Image happinessShower;

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
        happiness += 1;

        Logging.singleton.Log((Mathf.Round(happiness / 10f) * 10).ToString(), LogSeverity.Info);

        happinessShower.sprite = happinessSprites[Mathf.Clamp(happiness < 5 ? 0 : (int)Mathf.Round(happiness / 10f), 0, 9)];
    }

    /// <summary>
    /// Restarts the game scene
    /// </summary>
    public void Restart()
    {
        sceneLoader.LoadScene(1);
    }
}
