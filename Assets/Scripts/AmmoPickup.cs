//Pickup for players that gives players max ammo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : PickupBase
{

    //Max ammo is given to the player when stepped over
    public override void PickupPower(GameObject entity) {
        if (entity.TryGetComponent(out WeaponController weaponController)) {
            weaponController.AddAmmo((int)(weaponController.ActiveGunSlot.gun.stats.maxAmmo * 0.25f));
        }
        Destroy(gameObject);
    }
}
