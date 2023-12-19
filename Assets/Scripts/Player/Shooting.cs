/* Shooting.cs - HSSC-2
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
using Cinemachine;
using CsUtils.Systems.Logging;
using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct ShootingControls
{
    /// <summary>
    /// Key to hold for aiming
    /// </summary>
    public KeyCode aim;

    /// <summary>
    /// Key to press for shooting
    /// </summary>
    public KeyCode shoot;
}

public class Shooting : MonoBehaviour
{
    /// <summary>
    /// The different shooting controls
    /// </summary>
    public ShootingControls controls;

    /// <summary>
    /// The display for the gift counter
    /// </summary>
    public TextMeshProUGUI giftCounterShower;

    /// <summary>
    /// The camera handling the shooting
    /// </summary>
    public CinemachineVirtualCamera shootingCamera;

    /// <summary>
    /// Handles event firing for blending between cameras
    /// </summary>
    public CmBlendFinished blendHandler;

    /// <summary>
    /// The projectile to shoot
    /// </summary>
    public GameObject projectile;

    /// <summary>
    /// The sensitivity of the shooting camera
    /// </summary>
    public float sensitivity = 5f;

    /// <summary>
    /// The interval the player needs to wait before being able to shoot again
    /// </summary>
    public float shootingInterval = .2f;

    /// <summary>
    /// The amount of gifts the player has
    /// </summary>
    public int giftCounter;

    /// <summary>
    /// The speed at which the projectile is shot
    /// </summary>
    public float projectileSpeed;

    /// <summary>
    /// The timescale to use whilst aiming
    /// </summary>
    public float aimingTimescale;

    /// <summary>
    /// How long until the timescale returns back to normal whilst aiming
    /// </summary>
    public float aimingTimescaleMaxTime;

    /// <summary>
    /// Whether the player can control the shooting camera
    /// </summary>
    public bool canMoveCamera;

    /// <summary>
    /// Whether the player can shoot
    /// </summary>
    public bool canShoot;

    /// <summary>
    /// The x rotation of the camera
    /// </summary>
    float xRot;

    /// <summary>
    /// The y rotation of the camera
    /// </summary>
    float yRot = 90f;

    /// <summary>
    /// The normal value of the fixed delta time
    /// </summary>
    float normalFixedDeltatime;

    /// <summary>
    /// The id of the tween handling the timescale
    /// </summary>
    int timescaleTweenID;

    private void Start()
    {
        blendHandler.onBlendFinished += BlendFinished;
        normalFixedDeltatime = Time.fixedDeltaTime;
    }

    private void BlendFinished(CinemachineVirtualCameraBase cam)
    {
        canMoveCamera = true;
        Time.timeScale = aimingTimescale;
        Time.fixedDeltaTime *= aimingTimescale;

        LeanTween.cancel(timescaleTweenID);

        timescaleTweenID = LeanTween.delayedCall(aimingTimescaleMaxTime, ResetTimescale).setIgnoreTimeScale(true).uniqueId;
    }

    /// <summary>
    /// Resets the time scale back to its original value
    /// </summary>
    void ResetTimescale()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = normalFixedDeltatime;
    }

    private void Update()
    {
        HandleCameraRotation();
        HandleShooting();

        giftCounterShower.text = $"Gifts: {giftCounter}";
    }

    void HandleCameraRotation()
    {
        if (Input.GetKeyDown(controls.aim))
            shootingCamera.Priority = 5;

        if (Input.GetKeyUp(controls.aim))
        {
            shootingCamera.Priority = 1;
            canMoveCamera = false;

            ResetTimescale();
        }

        if (!canMoveCamera)
            return;

        yRot += sensitivity * Input.GetAxis("Mouse X");
        xRot -= sensitivity * Input.GetAxis("Mouse Y");

        xRot = Mathf.Clamp(xRot, -35f, 80f);

        shootingCamera.transform.eulerAngles = new Vector3(xRot, yRot, 0);
    }

    void HandleShooting()
    {
        if (!canMoveCamera || !canShoot)
            return;

        if (!Input.GetKeyDown(controls.shoot))
            return;

        if (giftCounter <= 0)
            return;

        GameObject obj = Instantiate(projectile);

        obj.transform.position = shootingCamera.transform.position;

        Rigidbody rb = obj.GetComponent<Rigidbody>();

        rb.AddForce(shootingCamera.transform.forward * projectileSpeed, ForceMode.VelocityChange);

        canShoot = false;
        giftCounter--;

        LeanTween.delayedCall(shootingInterval, () => canShoot = true).setIgnoreTimeScale(true);
    }

    private void Reset()
    {
        controls = new()
        {
            aim = KeyCode.Mouse1,
            shoot = KeyCode.Mouse0,
        };
    }
}