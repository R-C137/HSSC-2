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
 *                   
 *      [22/12/2023] - Added support for total distance flown (C137)
 *                   - Cursor is unlocked when the game over UI is displayed (C137)
 *                   - Happiness meter utility function support (C137)
 *                   - Trap activation SFX support (C137)
 *                   - Quit button support (C137)
 *                   - Pausing support (C137)
 *                   
 *      [23/12/2023] - Added support for gift collection SFX (C137)
 *                   - Ambient audios are now paused when the game is paused (C137)
 *                   - Added play time metric support (C137)
 *                   - Prevent game over menu bypass through pause menu (C137)
 */
using Cinemachine;
using CsUtils;
using System;
using System.Collections;
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

    /// <summary>
    /// The different SFXs to play when a trap is activated
    /// </summary>
    public AudioClip[] trapActivation;

    /// <summary>
    /// The different 'trash talk' SFX for Santa
    /// </summary>
    public AudioClip[] santaTrashTalk;

    /// <summary>
    /// The different 'trash talk' SFX the grinch
    /// </summary>
    public AudioClip[] grinchTrashTalk;
    
    /// <summary>
    /// The different SFXs to play when a gift is collected
    /// </summary>
    public AudioClip[] giftCollected;

    /// <summary>
    /// The SFX to play when an anvil is thrown
    /// </summary>
    public AudioClip anvilThrow;
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
    /// The audio sources handling ambient audio
    /// </summary>
    public AudioSource[] ambientAudio;

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
    /// The canvas for the pause UI
    /// </summary>
    public GameObject pauseCanvas;

    /// <summary>
    /// The key to press to open the pause menu
    /// </summary>
    public KeyCode pauseKey = KeyCode.Escape;

    /// <summary>
    /// Reference the to player model of the grinch
    /// </summary>
    public GameObject grinch;

    /// <summary>
    /// The amount of gifts the player has
    /// </summary>
    public int giftCounter;

    /// <summary>
    /// Whether the game is currently paused
    /// </summary>
    public bool isPaused;

    /// <summary>
    /// Whether the game is currently over
    /// </summary>
    public bool isGameOver;

    public float distanceFlown
    {
        get
        {
            return (PlayerMovement.singleton.player.transform.position - playerStartPos).x;
        }
    }

    /// <summary>
    /// Raised when a gift is delivered
    /// </summary>
    /// <param name="giftCounter">The current value of the gift counter</param>
    public delegate void GiftDelivery(ref int giftCounter);
    public event GiftDelivery onGiftDelivered;

    /// <summary>
    /// Raised when the game is pause
    /// </summary>
    /// <param name="doPausing">Whether the game was paused</param>
    public delegate void GamePaused(bool doPausing);
    public event GamePaused onGamePaused;

    /// <summary>
    /// The starting position of the player
    /// </summary>
    Vector3 playerStartPos;

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

        playerStartPos = PlayerMovement.singleton.player.transform.position;

        //This is no longer the player's first play through
        PlayerPrefs.SetInt("firstTimePlay", 0);

        StartCoroutine(PlayTimeHandler());
        StartCoroutine(DistanceFlownHandler());
    }

    IEnumerator DistanceFlownHandler()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(5f);
            PlayerPrefs.SetFloat("metrics.maxDistance", Mathf.Max(PlayerPrefs.GetFloat("metrics.maxDistance", 0), distanceFlown));
        }
    }

    IEnumerator PlayTimeHandler()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(60f);
            PlayerPrefs.SetInt("metrics.timePlayed", PlayerPrefs.GetInt("metrics.timePlayed", 0) + 1);
        }
    }

    private void Update()
    {

        giftCounterShower.text = giftCounter.ToString();

        if (Input.GetKeyDown(KeyCode.F11))
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;

        if(Input.GetKeyDown(pauseKey) && pauseCanvas != null)
        {
            if (isGameOver)
                return;
            pauseCanvas.SetActive(!isPaused);

            Pause(!isPaused);

            LockCursor(!isPaused);
        }
    }

    /// <summary>
    /// Sets the cursor lock mode
    /// </summary>
    /// <param name="lockState">Whether to lock the cursor</param>
    public void LockCursor(bool lockState)
    {
        Cursor.lockState = lockState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    /// <summary>
    /// Called when the game over menu needs to be shown
    /// </summary>
    [ContextMenu("Stimulate Game Over")]
    public void DoGameOver()
    {
        Pause(true);
        gameOverCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        isGameOver = true;
    }

    /// <summary>
    /// Returns the z rotation of the happiness meter
    /// </summary>
    /// <param name="input">The value of the happiness meter</param>
    /// <returns></returns>
    public float GetHappinessMeterRotation(float input)
    {
        // Define the input and output ranges
        float inMin = 0, inMax = 100, outMin = -90, outMax = 90;

        // Map the input to the output range
        float output = outMin + ((input - inMin) * (outMax - outMin)) / (inMax - inMin);

        return -output;
    }

    /// <summary>
    /// Loads the main menu
    /// </summary>
    public void MainMenu()
    {
        sceneLoader.LoadScene(0);
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
    /// Exits the game
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Sets the pause state of the game
    /// </summary>
    /// <param name="doPausing">Whether to pause the game</param>
    public void Pause(bool doPausing)
    {
        onGamePaused?.Invoke(doPausing);
        Time.timeScale = doPausing ? 0 : 1;

        isPaused = doPausing;

        if (doPausing)
        {
            foreach(AudioSource audio in ambientAudio)
            {
                audio.Pause();
            }
        }
        else
        {
            foreach (AudioSource audio in ambientAudio)
            {
                audio.Play();
            }
        }
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
