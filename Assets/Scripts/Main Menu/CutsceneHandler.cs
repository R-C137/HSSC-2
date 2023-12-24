/* CutsceneHandler.cs - HSSC-2
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
using CsUtils;
using CsUtils.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutsceneHandler : Singleton<CutsceneHandler>
{
    /// <summary>
    /// The fader used for the skip button
    /// </summary>
    public MultiFade fader;

    /// <summary>
    /// The cutscene to be played
    /// </summary>
    public VideoPlayer cutscene;

    /// <summary>
    /// The class handling the loading of the scene
    /// </summary>
    public SceneLoader sceneLoader;

    /// <summary>
    /// The button used to skip the cut scene
    /// </summary>
    public KeyCode skipButton;

    /// <summary>
    /// The length of the cutscene
    /// </summary>
    public float cutsceneTime;

    /// <summary>
    /// How long it takes for the key to fade
    /// </summary>
    public float keyFadeTime;

    /// <summary>
    /// How long the key is shown on screen before it starts fading out
    /// </summary>
    public float keyShowTime;

    /// <summary>
    /// Whether the cutscene is currently playing
    /// </summary>
    public bool doingCutscene;

    /// <summary>
    /// Whether a scene is already being loaded
    /// </summary>
    public bool sceneLoading;

    /// <summary>
    /// The id of the tween handling the cutscene
    /// </summary>
    int cutsceneTween;

    /// <summary>
    /// The id of the tween handling the fading of the key
    /// </summary>
    int keyFaderTwwen;

    public void Update()
    {
        if (!doingCutscene)
            return;

        if (Input.anyKeyDown)
        {
            LeanTween.cancel(keyFaderTwwen);

            fader.gameObject.SetActive(true);
            fader.SetAlpha(1f);

            keyFaderTwwen = LeanTween.value(1, 0, keyFadeTime)
                                     .setOnUpdate(v =>
                                     {
                                         if (fader == null)
                                             return;
                                         fader.SetAlpha(v);
                                     })
                                     .setOnComplete(() =>
                                     {
                                         if (fader == null || fader.gameObject == null)
                                             return;

                                         fader.gameObject.SetActive(false);
                                     })
                                     .setDelay(keyShowTime)
                                     .uniqueId;
        }

        if(Input.GetKeyUp(skipButton))
        {
            LeanTween.cancel(cutsceneTween, false);
            LoadScene();
        }
    }

    public void DoCutscene()
    {
        if (doingCutscene)
            return;

        doingCutscene = true;

        cutscene.gameObject.SetActive(true);

        cutscene.SetDirectAudioVolume(0, PlayerPrefs.GetFloat("settings.volume.general", 1));
        cutscene.targetTexture.Release();
        cutscene.Play();


        cutsceneTween = LeanTween.delayedCall(cutsceneTime, LoadScene).uniqueId;
    }

    public void LoadScene()
    {
        //#TODO: Pause the video player
        if (sceneLoading)
            return;

        cutscene.Pause();
        sceneLoader.LoadScene(1);

        sceneLoading = true;
    }
}
