//Damage specific entity

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : AreaOfEffect
{
    //Damage entity found from AreaOfEffect check
    public override void AffectEntity(GameObject entity, GameObject caster) {
        if (entity.TryGetComponent(out IHealth healthInterface)) {
            HitData data = new(entity.transform.position, Vector3.zero, valueAffectAmountPerIteration, DamageType.fire);
            healthInterface.Damage(data, caster);
        }
    }
}
