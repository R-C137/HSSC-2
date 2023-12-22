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
        giftsDeliveredShower.text = GiftCombo.singleton.giftDelivered.ToString();

        distanceFlownShower.text = $"{Utility.singleton.distanceFlown:N0}M";
    }

    /// <summary>
    /// Called when the restart button is clicked
    /// </summary>
    public void Restart()
    {
        Utility.singleton.Restart();
    }

}
