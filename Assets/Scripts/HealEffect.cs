//Heal effect for single entity from AreaOfEffect check radius function

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : AreaOfEffect {
    public override void AffectEntity(GameObject entity, GameObject caster) {
        if (entity.TryGetComponent(out IHealth healthInterface)) {
            healthInterface.Heal(valueAffectAmountPerIteration);
        }
    }
}
