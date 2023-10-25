//Base test ability

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Test Ability", menuName = "Abilities/Test Ability", order = 0)]
public class TestAbility : Ability
{
    public override bool Activate(GameObject caster) {
        return true;
    }

    public override void Deactivate(GameObject caster) {
    }
}
