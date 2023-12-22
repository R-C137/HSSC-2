/* GameOverHandler.cs - HSSC-2
 * 
 * Creation Date: 21/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [22/12/2023] - Initial implementation (C137)
 *                   - Exit button support (C137)
 *                   - Removed button handling (C137)
 *                   - Added support for happiness metric (C137)
 */
using TMPro;
using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    /// <summary>
    /// The shower for the distance flown
    /// </summary>
    public TextMeshProUGUI distanceFlownShower;

    /// <summary>
    /// The shower for the gifts delivered
    /// </summary>
    public TextMeshProUGUI giftsDeliveredShower;

    /// <summary>
    /// The rotational point for the happiness meter pointer
    /// </summary>
    public RectTransform happinessPointer;

    private void Update()
    {
        giftsDeliveredShower.text = $"Delivered: {GiftCombo.singleton.giftDelivered}";

        distanceFlownShower.text = $"{Utility.singleton.distanceFlown:N0}M";

        Vector3 rot = happinessPointer.eulerAngles;
        rot.z = Utility.singleton.GetHappinessMeterRotation(Mathf.Clamp(GiftCombo.singleton.happiness, 0, 100));
        happinessPointer.eulerAngles = rot;
    }
}
