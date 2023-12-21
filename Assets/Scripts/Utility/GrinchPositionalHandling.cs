/* PositionalHandling.cs - HSSC-2
 * 
 * Creation Date: 21/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [21/12/2023] - Initial implementation (C137)
 */
using CsUtils;
using System;
using UnityEngine;

[Serializable]
public struct GrinchPosition
{
    /// <summary>
    /// The position of the player
    /// </summary>
    public Transform position;

    /// <summary>
    /// The spawn rate of gifts/traps from the grinch
    /// </summary>
    public float objectSpawnRate;

    /// <summary>
    /// The speed at which the Grinch will transition to this position
    /// </summary>
    public float transitionalSpeed;
}

public class GrinchPositionalHandling : Singleton<GrinchPositionalHandling>
{
    /// <summary>
    /// The different positions the Grinch can be in
    /// </summary>
    public GrinchPosition[] positions;

    /// <summary>
    /// The script handling the Grinch's following of the player
    /// </summary>
    public AxisFollow grinchFollow;

    /// <summary>
    /// Reference to the Grinch to use for positional change
    /// </summary>
    public GameObject grinch;

    /// <summary>
    /// The current positional index of the Grinch
    /// </summary>
    public int currentPosition;

    /// <summary>
    /// The default parent of the Grinch before positional change
    /// </summary>
    Transform defaultParent;

    /// <summary>
    /// The id of the tween handling the positional change
    /// </summary>
    int positionalChangeTween;

    private void Start()
    {
        //Position needs to be set on the next frame or the movement will be weird for some reason
        LeanTween.delayedCall(0f, () => SetPosition(positions[currentPosition]));
    }

    [ContextMenu("Move Further")]
    public void MoveFurther()
    {
        if (currentPosition + 1 >= positions.Length)
            return;

        currentPosition++;

        SetPosition(positions[currentPosition]);
    }

    [ContextMenu("Move Closer")]
    public void MoveCloser()
    {
        if (currentPosition - 1 < 0)
            return;

        currentPosition--;

        SetPosition(positions[currentPosition]);
    }

    public void SetPosition(GrinchPosition position)
    {
        grinchFollow.enabled = false;

        defaultParent = grinchFollow.transform.parent;
        grinchFollow.transform.parent = position.position;

        LeanTween.cancel(positionalChangeTween, false);

        positionalChangeTween = LeanTween.moveLocal(grinchFollow.gameObject, Vector3.zero, 1f)
                 .setEase(LeanTweenType.easeInOutSine)
                 .setOnComplete(() =>
                 {
                     grinchFollow.offset = (grinchFollow.transform.position - grinchFollow.target.position).normalized * Vector3.Distance(grinchFollow.target.position, grinchFollow.transform.position);
                     grinchFollow.transform.parent = defaultParent;
                     grinchFollow.enabled = true;
                 }).uniqueId;

    }
}
