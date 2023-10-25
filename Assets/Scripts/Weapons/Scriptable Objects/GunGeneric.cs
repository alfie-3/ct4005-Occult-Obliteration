using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Weapons/Gun/Gun", order = 1)]
public class GunGeneric : GunBase {
    public GunGeneric() {
        gunType = GunType.Rifle;
    }

    public override void Fire(WeaponController controller) {
        //Fires raycast and sorts order of array based on the distance of the target from the player
        if(controller == null) return;

        Vector3 directionWithSpread = Quaternion.AngleAxis(Random.Range(-controller.GetSpread(), controller.GetSpread()), controller.transform.up) * controller.transform.forward;

        controller.FireRaycast(directionWithSpread);
    }
}