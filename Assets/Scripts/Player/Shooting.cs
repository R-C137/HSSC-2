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
 *                   - Made ResetTimescale() a public function (C137)
 *                   - Moved gift counting and display to Utility script (C137)
 *                   
 *      [21/12/2023] - Added support for new SFX handling system (C137)
 *      [22/12/2023] - Game pausing support (C137)
 *      [23/12/2023] - Made class a singleton (C137)
 *                   - Added anvil shooting support (C137)
 */
using Cinemachine;
using CsUtils;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
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

[System.Serializable]
public struct AnvilSprites
{
    /// <summary>
    /// Sprite to use when the anvil is active
    /// </summary>
    public Sprite active;

    /// <summary>
    /// Sprite to use when the anvil is inactive
    /// </summary>
    public Sprite inactive;
}

public class Shooting : Singleton<Shooting>
{
    /// <summary>
    /// The different shooting controls
    /// </summary>
    public ShootingControls controls;

    /// <summary>
    /// The shower for the anvil icon
    /// </summary>
    public Image anvilIcon;

    /// <summary>
    /// The different sprites to use for the anvil
    /// </summary>
    public AnvilSprites anvilSprites;

    /// <summary>
    /// The audio source used to play the SFX
    /// </summary>
    public AudioSource audioSource;

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
    /// The projectile to use when shooting an anvil
    /// </summary>
    public GameObject anvilProjectile;

    /// <summary>
    /// The sensitivity of the shooting camera
    /// </summary>
    public float sensitivity = 5f;

    /// <summary>
    /// The interval the player needs to wait before being able to shoot again
    /// </summary>
    public float shootingInterval = .2f;

    /// <summary>
    /// The speed at which the projectile is shot
    /// </summary>
    public float projectileSpeed;

    /// <summary>
    /// The speed at which the anvil projectile is shot
    /// </summary>
    public float anvilProjectileSpeed;

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
    /// Whether the player is currently shooting an anvil
    /// </summary>
    public bool shootingAnvil;

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
        Utility.singleton.onGamePaused += HandlePausing;

        normalFixedDeltatime = Time.fixedDeltaTime;
    }

    private void HandlePausing(bool doPausing)
    {
        ResetTimescale();
        canMoveCamera = !doPausing;
        canShoot = !doPausing;
    }

    private void BlendFinished(CinemachineVirtualCameraBase cam)
    {
        if (Utility.singleton.isPaused)
            return;

        canMoveCamera = true;
        Time.timeScale = aimingTimescale;
        Time.fixedDeltaTime *= aimingTimescale;

        LeanTween.cancel(timescaleTweenID);

        timescaleTweenID = LeanTween.delayedCall(aimingTimescaleMaxTime, () => 
        { 
            if (!Utility.singleton.isPaused)
                ResetTimescale(); 

        }).setIgnoreTimeScale(true).uniqueId;
    }

    /// <summary>
    /// Resets the time scale back to its original value
    /// </summary>
    public void ResetTimescale()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = normalFixedDeltatime;
    }

    private void Update()
    {
        if (Utility.singleton.isPaused)
            return;

        HandleCameraRotation();
        HandleShooting();

        anvilIcon.sprite = shootingAnvil ? anvilSprites.active : anvilSprites.inactive;
    }

    void HandleCameraRotation()
    {
        if (Input.GetKeyDown(controls.aim))
        {
            shootingCamera.Priority = 5;
        }

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

        if (shootingAnvil)
        {
            GameObject anvilObj = Instantiate(anvilProjectile);
            anvilObj.transform.position = shootingCamera.transform.position;

            Rigidbody anvilRigidBody = anvilObj.GetComponent<Rigidbody>();
            anvilRigidBody.AddForce(shootingCamera.transform.forward * anvilProjectileSpeed, ForceMode.VelocityChange);

            audioSource.clip = Utility.singleton.commonSFX.anvilThrow;
            audioSource.Play();

            shootingAnvil = false;

            return;
        }

        if (Utility.singleton.giftCounter <= 0)
            return;

        GameObject projectileObj = Instantiate(projectile);
        projectileObj.transform.position = shootingCamera.transform.position;

        Rigidbody projectileRigidBody = projectileObj.GetComponent<Rigidbody>();
        projectileRigidBody.AddForce(shootingCamera.transform.forward * projectileSpeed, ForceMode.VelocityChange);

        canShoot = false;
        Utility.singleton.giftCounter--;

        //Play SFX
        audioSource.clip = Utility.singleton.commonSFX.projectileShoot[Random.Range(0, Utility.singleton.commonSFX.projectileShoot.Length)];
        audioSource.Play();

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
