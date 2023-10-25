using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shotgun", menuName = "Weapons/Gun/Shotgun", order = 1)]
[System.Serializable]
public class GunShotgun : GunBase {
    public GunShotgun() {
        stats = new ShotgunStats();
        stats.spread.x = 45;
        gunType = GunType.Shotgun;
    }

    public override void Fire(WeaponController controller) {
        ShotgunStats shotgun = stats as ShotgunStats;
        int shots = shotgun.shotAmount;
        var currentAngle = controller.GetSpread() / 2;

        for (int i = 0; i < shots; i++) {

            //Angle is calculated for the shotgun spread, based on the angle devided by the spray so the shots are dispersed evenly.
            Vector3 shotDir = Quaternion.AngleAxis(currentAngle, controller.transform.up) * controller.transform.forward;

            currentAngle -= (controller.GetSpread() / shotgun.shotAmount);

            Vector3 directionWithSpread = Quaternion.AngleAxis(Random.Range(-shotgun.angleDither, shotgun.angleDither), controller.transform.up) * shotDir;

            controller.FireRaycast(directionWithSpread);
        }
    }
}

[System.Serializable]
public class ShotgunStats : GunStats {
    [Header("Shotgun Specific Stats")]
    public int shotAmount = 10;
    [Range(1f, 10f)]
    public float angleDither = 5;
}
