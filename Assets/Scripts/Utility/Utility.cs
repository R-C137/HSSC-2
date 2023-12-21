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
 *     
 *      [21/12/2023] - Camera effect support (C137)
 *                   - Added references to commonly used SFX (C137)
 *                   - Cursor lock key bind (C137)
 */
using Cinemachine;
using CsUtils;
using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct CommonSFX
{
    /// <summary>
    /// SFX played when a clickable UI element is hovered
    /// </summary>
    public AudioClip UIHover;

    /// <summary>
    /// SFX played when a UI element is clicked
    /// </summary>
    public AudioClip UIClick;

    /// <summary>
    /// The different SFXs to play when a projectile is shot
    /// </summary>
    public AudioClip[] projectileShoot;

}

public class Utility : Singleton<Utility>
{
    /// <summary>
    /// Reference to common SFXs used
    /// </summary>
    public CommonSFX commonSFX;

    /// <summary>
    /// Class handling the scene loading animation
    /// </summary>
    public SceneLoader sceneLoader;

    /// <summary>
    /// Reference to the cinemachine brain of the scene
    /// </summary>
    public CinemachineBrain cinemachineBrain;

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

    /// <summary>
    /// The id of the tween handling the camera shake
    /// </summary>
    int cameraShakeTweenID;

    /// <summary>
    /// The id of the tween handling the fov change
    /// </summary>
    int fovChangeTweenID;

    /// <summary>
    /// The id of the tween handling the fov change back
    /// </summary>
    int fovChangeBackTwwen;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        giftCounterShower.text = $"Gifts: {giftCounter}";

        if (Input.GetKeyDown(KeyCode.F11))
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
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
        GrinchBehaviour.singleton.spawnObjects = false;
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

    /// <summary>
    /// Shakes the camera
    /// </summary>
    /// <param name="intensity">How intensely the camera should shake</param>
    /// <param name="time">How long should the shaking last</param>
    public void ShakeCamera(float intensity = 5f, float time = 1f)
    {
        LeanTween.cancel(cameraShakeTweenID);

        var perlin = (cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cameraShakeTweenID = LeanTween.value(intensity, 0, time)
            .setOnUpdate((v) => perlin.m_AmplitudeGain = v).uniqueId;
        
    }

    /// <summary>
    /// Changes the FOV of the active camera to a set value
    /// </summary>
    /// <param name="fov">The fov to set the camera to</param>
    /// <param name="time">How long it takes for the fov to change</param>
    /// <param name="stayTime">How long should the new fov remain before changing back to its original value</param>
    /// <param name="changeBackTime">How long it takes for the fov to change back to its original value</param>
    public void ChangeFOV(float fov, float time, float stayTime, float changeBackTime = 0)
    {
        changeBackTime = changeBackTime == 0 ? time : changeBackTime;

        LeanTween.cancel(fovChangeTweenID);

        float startingFOV = (cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera).m_Lens.FieldOfView;

        fovChangeTweenID = LeanTween.value(startingFOV, fov, time)
                 .setOnUpdate(v => (cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera).m_Lens.FieldOfView = v)
                 .setOnComplete(() =>
                 {
                     LeanTween.cancel(fovChangeBackTwwen);

                     fovChangeBackTwwen = LeanTween.value(fov, startingFOV, changeBackTime)
                              .setDelay(stayTime)
                              .setOnUpdate(v => (cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera).m_Lens.FieldOfView = v).uniqueId;
                 }).uniqueId;
    }
}
