/* TrainTrapPart.cs - HSSC-2
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
using UnityEngine;

public class TrainTrapPart : TrapBehaviour
{
    public override void Start()
    {
        
    }

    public override void TrapHit()
    {
        transform.parent.GetComponent<TrapBehaviour>().TrapHit();
    }

    public override void TrapShot()
    {
        transform.parent.GetComponent<TrapBehaviour>().TrapShot();
    }
}
