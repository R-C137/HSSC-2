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
 *      [22/12/2023] - Proper happiness meter (C137)
 *                   - Improved combo slider (C137)
 */
using CsUtils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct FillImageSprite
{
    /// <summary>
    /// The sprite to use when the combo is low
    /// </summary>
    public Sprite low;

    /// <summary>
    /// The sprite to use when the combo is high
    /// </summary>
    public Sprite high;
}

public class GiftCombo : Singleton<GiftCombo>
{
    /// <summary>
    /// The fill image used to display the combo
    /// </summary>
    public Image comboFillImage;

    /// <summary>
    /// The different sprites to used for the values of the combo
    /// </summary>
    public FillImageSprite comboFillImgeSprites;

    /// <summary>
    /// The curve used to control the increase in the number of gifts to be delivered for the combo to increase
    /// </summary>
    public AnimationCurve comboIncreaseCurve;

    /// <summary>
    /// The display used to show the value of the multiplier
    /// </summary>
    public TextMeshProUGUI multiplierDisplay;

    /// <summary>
    /// The rotational point for the happiness meter pointer
    /// </summary>
    public RectTransform happinessPointer;

    /// <summary>
    /// The total amount of gifts delivered
    /// </summary>
    public float giftDelivered;

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
    /// The delay for decaying the happiness after it has been increased
    /// </summary>
    public float happinessDecayDelay;

    /// <summary>
    /// How long it takes for the combo to decay
    /// </summary>
    public float comboDecaySpeed;

    /// <summary>
    /// How long it takes for the happiness to decay
    /// </summary>
    public float happinessDecaySpeed;

    /// <summary>
    /// Whether to decay the combo
    /// </summary>
    public bool doComboDecay;

    /// <summary>
    /// Whether to decay the happiness
    /// </summary>
    public bool doHappinessDecay;

    /// <summary>
    /// The id of the tween handling the combo
    /// </summary>
    int comboTweenID;

    /// <summary>
    /// The id of the tween handling the happiness
    /// </summary>
    int happinessTweenID;

    private void Start()
    {
        Utility.singleton.onGiftDelivered += GiftDelivered;
    }

    private void Update()
    {
        multiplierDisplay.text = $"X{happinessMultiplier}";

        Vector3 rot = happinessPointer.eulerAngles;

        rot.z = Utility.singleton.GetHappinessMeterRotation(Mathf.Clamp(happiness, 0, 100));

        happinessPointer.eulerAngles = rot;

        comboFillImage.sprite = happinessMultiplier > 10 ? comboFillImgeSprites.high : comboFillImgeSprites.low;

        if (doHappinessDecay)
            happiness = Mathf.Clamp(happiness - (100 / happinessDecaySpeed * Time.deltaTime), 0, 100);

        if (!doComboDecay)
            return;

        comboFillImage.fillAmount = Mathf.Clamp01(comboFillImage.fillAmount - ((1 / comboDecaySpeed) * Time.deltaTime));

        if(comboFillImage.fillAmount == 0)
        {
            if (happinessMultiplier > 1)
            {
                happinessMultiplier--;
                comboFillImage.fillAmount = 1;
            }
        }

    }

    private void GiftDelivered(ref int giftCounter)
    {
        doComboDecay = false;

        LeanTween.cancel(comboTweenID);
        comboTweenID = LeanTween.delayedCall(comboDecayDelay, () => doComboDecay = true).uniqueId;

        doHappinessDecay = false;
        LeanTween.cancel(happinessTweenID);
        happinessTweenID = LeanTween.delayedCall(happinessDecayDelay, () => doHappinessDecay = true).uniqueId;

        comboFillImage.fillAmount += 1 / comboIncreaseCurve.Evaluate(happinessMultiplier);

        if(comboFillImage.fillAmount >= 1)
        {
            happinessMultiplier += 1;

            comboFillImage.fillAmount = 0;
        }

        happiness += 1 * happinessMultiplier;

        giftDelivered++;
    }
}
