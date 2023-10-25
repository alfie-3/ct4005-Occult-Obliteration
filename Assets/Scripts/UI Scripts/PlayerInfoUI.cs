//Holds information for individual player UI info

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq.Expressions;

public class PlayerInfoUI : MonoBehaviour
{
    [Header("Icon")]
    [SerializeField] Image iconImage;
    [SerializeField] Image backGround;
    [SerializeField] Image numberBackground;
    [Header("Health")]
    [SerializeField] Slider healthSlider;
    [SerializeField] Image healthSliderFill;
    [SerializeField] Image healthSliderBackground;
    [Header("Stats")]
    [SerializeField] TextMeshProUGUI className;
    [SerializeField] TextMeshProUGUI souls;
    [Header("Guns")]
    [SerializeField] TextMeshProUGUI currentMagText;
    [SerializeField] TextMeshProUGUI currentReserve;
    [SerializeField] Image gunIcon;
    [SerializeField] TextMeshProUGUI gunName;
    [SerializeField] Image secondaryGun;
    [SerializeField] TextMeshProUGUI gunSwapButton;
    [SerializeField] Image divider;
    [SerializeField] Image weaponBackgroundImage;

    public void InitUI(PlayerManager manager) {
        PlayerDetails details = manager.GetPlayerDetails();

        SetColors(details.playerClass);

        iconImage.sprite = details.playerClass.ClassInfo.selectImage;

        className.text = manager.PlayerClass.ClassInfo.name;

        if (manager.GetPlayerDetails().nextGun != null) {
            secondaryGun.gameObject.SetActive(true);
        }
        else
            secondaryGun.gameObject.SetActive(false);

    }

    //Set colour for playerInfoUI
    private void SetColors(PlayerClass playerClass)
    {
        backGround.color = playerClass.classColour;
        healthSliderFill.color = playerClass.classColour;
        Color darkerColour = playerClass.classColour * healthSliderBackground.color.grayscale;
        healthSliderBackground.color = darkerColour;
        numberBackground.color = darkerColour;
        divider.color = darkerColour;

        weaponBackgroundImage.material = new(weaponBackgroundImage.material);

        weaponBackgroundImage.material.SetColor("_Colour_A", playerClass.classColour);
        Color noOpacityCol = playerClass.classColour;
        noOpacityCol.a = 0;
        weaponBackgroundImage.material.SetColor("_Colour_B", noOpacityCol);
    }

    //Sets players UI information to display what a player needs to know.
    public void UpdatePlayerInfo(PlayerManager manager)
    {
       PlayerDetails details = manager.GetPlayerDetails();

        healthSlider.maxValue = details.maxHealth;
        healthSlider.value = details.health;

        currentMagText.text = $"{details.currentAmmo} /";
        currentReserve.text = details.reserveAmmo.ToString();

        souls.text = $"<sprite=0>{manager.souls}";

        if (details.nextGun != null) {
            gunSwapButton.gameObject.SetActive(true);
            secondaryGun.gameObject.SetActive(true);
            secondaryGun.sprite = details.nextGun.gunData.gunIcon;

            if (manager.input.currentControlScheme == "KeyboardMouse") {
                gunSwapButton.text = "<sprite=\"KeyboardMouseIconSheet\" name=\"Keyboard_q\">";
            }
            else {
                gunSwapButton.text = "<sprite=\"XboxIconSpriteSheet\" name=\"Gamepad_buttonNorth\">";
            }
        }
        else gunSwapButton.gameObject.SetActive(false);

        if (details.currentGun == null)
            return;

        if (gunName.text != details.currentGun.name) {
            gunName.text = details.currentGun.gunData.gunName;
            gunIcon.sprite = details.currentGun.gunData.gunIcon;
        }
    }
}
