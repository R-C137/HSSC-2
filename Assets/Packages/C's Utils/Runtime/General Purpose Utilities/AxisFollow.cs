/* AxisFollow.cs - C's Utils
 * 
 * Follow an object along select axes, using an offset with smoothing
 * 
 * 
 * Creation Date: 13/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [13/12/2023] - Initial implementation (C137)
 *      
 *      [16/12/2023] - Fixed script name in the meta data (C137)
 *                   - Removed unnecessary using statements (C137)
 *      
 *      [18/12/2023] - Fixed animation curve being null on Reset() (C137)
 *                   - Fixed easing not following smoothing (C137)
 *                   
 *      [19/12/2023] - Changed the movemeant to run on FixedUpdate to make movemeant smoother (Archetype)
 *      
 *      [20/12/2023] - Changed movemeant lerp to use deltaTime so it will slow down properly (Archetype)
 *                   - Added functions for changing distance to player (Archetype)
 *                   
 *      [21/12/2023] - Smoothing is now done per axis (C137)
 *                   - Reverted changes 1,2 (20/12/2023)
 */
using System;
using UnityEngine;

[Serializable]
public struct FollowAxes
{
    /// <summary>
    /// Which of the axes to follow
    /// </summary>
    public bool followX, followY, followZ;
}

[Serializable]
public struct SmoothingAxes
{
    /// <summary>
    /// Which of the axes to apply smoothing on
    /// </summary>
    public bool smoothX, smoothY, smoothZ;
}

public class AxisFollow : MonoBehaviour
{
    /// <summary>
    /// The target to follow
    /// </summary>
    public Transform target;

    /// <summary>
    /// How smooth should the follow be. Set to 1 to disable smoothing
    /// </summary>
    public float smoothing = 1;

    /// <summary>
    /// The easing to use
    /// </summary>
    public AnimationCurve easing;

    /// <summary>
    /// The offset by which to follow the target
    /// </summary>
    public Vector3 offset;

    /// <summary>
    /// The axes to follow
    /// </summary>
    public FollowAxes axes;

    /// <summary>
    /// The axes to apply smoothing on
    /// </summary>
    public SmoothingAxes smoothAxes;

    /// <summary>
    /// Whether to automatically set the offset when a new target is added
    /// </summary>
    public bool autoOffset = true;

    /// <summary>
    /// Whether to use the easing for smoothing
    /// </summary>
    public bool useEasing;

    /// <summary>
    /// Internal reference to determine when the follow target has changed
    /// </summary>
    Transform _oldTarget;

    public void FixedUpdate()
    {
        FollowTarget();
    }

    /// <summary>
    /// Handles the following of the target
    /// </summary>
    public void FollowTarget()
    {
        Vector3 pos = transform.position;

        if (axes.followX)
            pos.x = target.position.x + offset.x;
        if (axes.followY)
            pos.y = target.position.y + offset.y;
        if (axes.followZ)
            pos.z = target.position.z + offset.z;

        transform.position = new Vector3(
            smoothAxes.smoothX ? Mathf.Lerp(transform.position.x, pos.x, easing.Evaluate(smoothing) * Time.timeScale) : pos.x,
            smoothAxes.smoothY ? Mathf.Lerp(transform.position.y, pos.y, easing.Evaluate(smoothing) * Time.timeScale) : pos.y,
            smoothAxes.smoothZ ? Mathf.Lerp(transform.position.z, pos.z, easing.Evaluate(smoothing) * Time.timeScale) : pos.z);

        //transform.position = Vector3.Lerp(transform.position, pos, easing.Evaluate(smoothing) * Time.timeScale);
    }

    /// <summary>
    /// Set the proper default values
    /// </summary>
    private void Reset()
    {
        easing = new AnimationCurve();
        easing.AddKey(0, 1);
        easing.AddKey(1, 1);
    }

    /// <summary>
    /// Handles auto offsetting and prevents illegal values
    /// </summary>
    private void OnValidate()
    {
        smoothing = Mathf.Clamp01(smoothing);

        if (autoOffset && target != null && _oldTarget != target)
        {
            offset = (transform.position - target.position).normalized * Vector3.Distance(target.position, transform.position);
            autoOffset = false;
        }

        _oldTarget = target;

        if(useEasing)
        {
            Keyframe[] keys = easing.keys;

            keys[^1] = new Keyframe(1, smoothing);

            easing.keys = keys;
        }
        else
        {
            easing.ClearKeys();
            easing.AddKey(0, smoothing);
            easing.AddKey(1, smoothing);
        }
    }
}