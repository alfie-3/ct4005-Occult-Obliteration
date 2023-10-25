using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class WeaponStatCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gunName, firingType, weaponType, damage, pierce, speed, cost, prompt;

    public void Init(GunBase gun, PlayerInput input, bool displayPrice) {
        gunName.text = gun.gunData.gunName.ToString();
        firingType.text = gun.stats.firingType.ToString();
        weaponType.text = gun.gunType.ToString();
        damage.text = gun.stats.damage.ToString();
        pierce.text = gun.stats.pierceAmount.ToString();
        speed.text = "N/A";
        cost.text = @$"<sprite=""Soul"" index=0>{gun.gunData.price}";

        if (!displayPrice)
            cost.enabled = false;

        InputBinding binding = input.actions["Interact"].bindings[(int)InputIconTool.RetrieveDeviceType(input.currentControlScheme)];
        prompt.text = $"{InputIconTool.RetrieveSprite(binding, input.currentControlScheme)}";
    }
}
