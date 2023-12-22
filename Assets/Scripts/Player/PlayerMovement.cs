/* PlayerMovement.cs - HSSC-2
 * 
 * Creation Date: 18/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137, Archetype
 * 
 * Changes: 
 *      [18/12/2023] - Initial implementation (C137)
 *                   - Added clamps (Archetype)
 *      
 *      [19/12/2023] - Added a boolean to disable movement (C137)
 *                   - Made class a singleton (C137)
 *                   
 *      [21/12/2023] - Vertical movement decay is now optional (C137)
 *      
 *      [22/12/2023] - Game pausing support (C137)
 */
using CsUtils;
using UnityEngine;

[System.Serializable]
public struct PlayerMovementControls
{
    /// <summary>
    /// Key to hold to go up or release to go down
    /// </summary>
    public KeyCode up;

    /// <summary>
    /// Key to hold to go right
    /// </summary>
    public KeyCode right;

    /// <summary>
    /// Key to hold to go left
    /// </summary>
    public KeyCode left;
}

public class PlayerMovement : Singleton<PlayerMovement>
{
    /// <summary>
    /// The controls for the player movement
    /// </summary>
    public PlayerMovementControls controls;

    /// <summary>
    /// The transform on which the movement should affect
    /// </summary>
    public Transform player;

    /// <summary>
    /// The speed at which the player moves forward
    /// </summary>
    public float forwardSpeed;

    /// <summary>
    /// The speed at which the player moves right and left
    /// </summary>
    public float lateralSpeed;

    /// <summary>
    /// The speed at which the player moves up and down
    /// </summary>
    public float verticalSpeed;

    /// <summary>
    /// Whether the player should move forward
    /// </summary>
    public bool moveForward = true;

    /// <summary>
    /// Whether to decay the vertical positioning of the player
    /// </summary>
    public bool doVerticalDecay = true;

    /// <summary>
    /// Whether to do any movement at all
    /// </summary>
    public bool doMovement = true;

    /// <summary>
    /// Clamp limits
    /// </summary>
    public float minY = -0.5f, maxY = 40f, minZ = -23f, maxZ = 23f;

    private void Start()
    {
        Utility.singleton.onGamePaused += HandlePausing;
    }

    private void HandlePausing(bool doPausing)
    {
        doMovement = !doPausing;
        doVerticalDecay = !doPausing;
        moveForward = !doPausing;
    }

    private void FixedUpdate()
    {
        if (!doMovement)
            return;

        HandleMovement();
        Clamp();
    }

    void Clamp()
    {
        // Get the current position of the character
        Vector3 currentPosition = player.position;

        // Clamp posiitons
        currentPosition.y = Mathf.Clamp(currentPosition.y, minY, maxY);
        currentPosition.z = Mathf.Clamp(currentPosition.z, minZ, maxZ);
        player.position = currentPosition;
    }

    public void HandleMovement()
    {
        Vector3 pos = player.position;

        if (moveForward)
            pos.x += forwardSpeed * Time.deltaTime;

        if (Input.GetKey(controls.up))
            pos.y += verticalSpeed * Time.deltaTime;
        else if (doVerticalDecay)
            pos.y -= verticalSpeed * Time.deltaTime;

        if (Input.GetKey(controls.right))
            pos.z -= lateralSpeed * Time.deltaTime;

        if (Input.GetKey(controls.left))
            pos.z += lateralSpeed * Time.deltaTime;

        player.position = pos;
    }

    private void Reset()
    {
        player = transform;

        controls = new()
        {
            up = KeyCode.Space,
            right = KeyCode.D,
            left = KeyCode.A,
        };
    }
}
