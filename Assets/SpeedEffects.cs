/* SpeedEffects.cs - HSSC-2
 * 
 * Creation Date: 20/12/2023
 * Authors: Archetype
 * Original: Archetype
 * 
 * Edited By: Archetype
 * 
 * Changes: 
 *      [20/12/2023] - Initial implementation (Archetype)
 */
using System.Collections;
using UnityEngine;
using Cinemachine;

public class SpeedEffects : MonoBehaviour
{
    /// <summary>
    /// The Follow camera (will be shaken)
    /// </summary>
    public CinemachineVirtualCamera vCamera;

    /// <summary>
    /// FOV states to indicate speeding and normal
    /// </summary>
    public float zoomingFOV = 80, normalFOV = 60;
    public float zoomSpeed = 1;

    /// <summary>
    /// If the FOV is zooming in and out
    /// </summary>
    bool zooming;

    /// <summary>
    /// The coroutines being declared so they can be canceled
    /// </summary>
    Coroutine _zoomingC, _shakingC;

    /// <summary>
    /// Values that will determine screen shake duration and intensity
    /// </summary>
    public float shakeDuration = 1f;
    public float shakeAmount = 0.7f;

    /// <summary>
    /// Whether or not the screen is currently shaking
    /// </summary>
    private bool isShaking = false;

    /// <summary>
    /// The camLookAt transform that the cinemachine uses and a stable version to center the screen shake
    /// </summary>
    public Transform camLookAt, stableLookAt;

    public void ZoomSpeed()
    {
        if (isShaking)
        {
            StopCoroutine(_shakingC);
            camLookAt.position = stableLookAt.position;
            isShaking = false;
        }

        if (!zooming)
        {
            zooming = true;
            _zoomingC = StartCoroutine(ChangeFOV(vCamera, zoomingFOV, zoomSpeed));
        }
    }

    void Update()
    {
        if (isShaking)
        {
            // Generate a random offset within the specified range
            Vector3 offset = new Vector3(Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount), 0f);

            // Apply the offset to the camera's position
            camLookAt.position = stableLookAt.position + offset;
        }
    }

    public void TriggerScreenShake()
    {
        if (zooming)
        {
            StopCoroutine(_zoomingC);
            vCamera.m_Lens.FieldOfView = normalFOV;
            zooming = false;
        }

        if (!isShaking)
        {
            // Start the screen shake coroutine
            _shakingC = StartCoroutine(Shake());
        }
    }

    IEnumerator Shake()
    {
        isShaking = true;

        // Wait for the specified duration
        yield return new WaitForSeconds(shakeDuration);

        // Reset the camera position after the screen shake is complete
        camLookAt.position = stableLookAt.position;
        isShaking = false;
    }

    IEnumerator ChangeFOV(CinemachineVirtualCamera cam, float endFOV, float duration)
    {
        float startFOV = cam.m_Lens.FieldOfView;
        float time = 0;
        while (time < duration)
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(startFOV, endFOV, time / duration);
            yield return null;
            time += Time.deltaTime;
        }

        yield return new WaitForSeconds(1f);

        startFOV = cam.m_Lens.FieldOfView;
        time = 0;
        while (time < duration)
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(startFOV, normalFOV, time / duration);
            yield return null;
            time += Time.deltaTime;
        }

        zooming = false;
    }
}