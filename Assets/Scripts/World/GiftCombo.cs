/* GiftCombo.cs - HSSC-2
 * 
 * Creation Date: 20/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [20/12/2023] - Initial implementation (C137)
 */
using CsUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftCombo : Singleton<GiftCombo>
{
    /// <summary>
    /// The slider used to display the combo
    /// </summary>
    public Slider comboSlider;

    /// <summary>
    /// The curve used to control the increase in the number of gifts to be delivered for the combo to increase
    /// </summary>
    public AnimationCurve comboIncreaseCurve;

    /// <summary>
    /// The display used to show the value of the multiplier
    /// </summary>
    public TextMeshProUGUI multiplierDisplay;

    /// <summary>
    /// The overall happiness of the world
    /// </summary>
    public float happiness;

    /// <summary>
    /// The current value of the happiness multiplier combo
    /// </summary>
    public float happinessMultiplier;

    /// <summary>
    /// The delay before decaying the combo after it has been increased
    /// </summary>
    public float comboDecayDelay;

    /// <summary>
    /// How long it takes for the combo to decay
    /// </summary>
    public float comboDecaySpeed;

    /// <summary>
    /// Whether to do the decay of the combo
    /// </summary>
    public bool doComboDecay;

    /// <summary>
    /// The id of the tween handling the combo
    /// </summary>
    int comboTweenID;

    private void Start()
    {
        Utility.singleton.onGiftDelivered += GiftDelivered;
    }

    private void Update()
    {
        multiplierDisplay.text = $"X{happinessMultiplier}";

        if (!doComboDecay)
            return;

        comboSlider.value = Mathf.Clamp01(comboSlider.value - ((1 / comboDecaySpeed) * Time.deltaTime));

        if(comboSlider.value == 0)
        {
            if (happinessMultiplier > 1)
            {
                happinessMultiplier--;
                comboSlider.value = 1;
            }
        }
    }

    private void GiftDelivered(ref int giftCounter)
    {
        doComboDecay = false;

        LeanTween.cancel(comboTweenID);
        comboTweenID = LeanTween.delayedCall(comboDecayDelay, () => doComboDecay = true).uniqueId;

        comboSlider.value += 1 / comboIncreaseCurve.Evaluate(happinessMultiplier);

        if(comboSlider.value >= 1)
        {
            happinessMultiplier += 1;

            comboSlider.value = 0;
        }

        happiness += 1 * happinessMultiplier;
    }
}
