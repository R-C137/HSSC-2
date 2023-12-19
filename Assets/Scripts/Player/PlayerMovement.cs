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
 *      [18/12/2023] - Added clamps (Archetype)
 */
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

public class PlayerMovement : MonoBehaviour
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
    /// Clamp limits
    /// </summary>
    public float minY = -0.5f, maxY = 40f, minZ = -23f, maxZ = 23f;

    private void FixedUpdate()
    {
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
        else
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
