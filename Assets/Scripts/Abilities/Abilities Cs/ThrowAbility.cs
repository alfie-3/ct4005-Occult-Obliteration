//Ability that throws any grenade prefab

using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[CreateAssetMenu(fileName = "New Grenade Ability", menuName = "Abilities/Throw Ability", order = 1)]
public class ThrowAbility : Ability {
    [SerializeField] GameObject throwablePrefab;
    [SerializeField] float forwardObjectSpawn;

    //Shows grenade trajectory and explosion radius when the grenade button is held
    public override void ButtonHeld(GameObject caster) {
        base.ButtonHeld(caster);
    }

    //Grenade thrown on ability button release
    public override bool Activate(GameObject caster) {
        base.Activate(caster);
        ThrowGrenade(caster);

        return true;
    }

    //Adds initial force to the grenade
    private void ThrowGrenade(GameObject caster) {
        RequiresCaster throwable = Instantiate(throwablePrefab, caster.transform.position+(caster.transform.forward*forwardObjectSpawn), Quaternion.identity).GetComponent<RequiresCaster>();
        throwable.Init(caster);
        throwable.transform.forward = caster.transform.forward;
    }

    [Serializable]
    public class LineRendererSettings {
        public Material material;
        public Gradient color;
        public float width;
    }
}

