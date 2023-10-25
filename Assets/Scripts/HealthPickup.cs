//Object that gives health to player when walked over

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthPickup : PickupBase
{
    [SerializeField] int healthGain;

    //Player gains health
    public override void PickupPower(GameObject entity) {
        if (entity.TryGetComponent(out IHealth healthInterface)) {
            healthInterface.Heal(healthGain);
        }
        Destroy(gameObject);
    }
}
