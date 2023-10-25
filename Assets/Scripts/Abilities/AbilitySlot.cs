//Holds abilities for players to use

using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class AbilitySlot {
    public Ability ability;

    [SerializeField]
    enum AbilityState {
        ready,
        active,
        cooldown
    }

    [SerializeField, ReadOnly] AbilityState state = AbilityState.ready;
    //Perform held ability if it's ready
    public void ButtonHeld(GameObject caster) {
        if (state == AbilityState.ready) {
            ability.ButtonHeld(caster);
        }
    }

    //Perform pressed ability if it's ready
    public async void ActivateAbility(GameObject caster) {
        if (state == AbilityState.ready) {
            await PerformAbility(caster);
        }
    }

    //The ability is activated and set "Active"
    public async Task PerformAbility(GameObject caster) {

        state = AbilityState.active;
        bool didActivate = ability.Activate(caster);

        if (didActivate) {
            await Task.Delay((int)(ability.duration * 1000));

            ability.Deactivate(caster);
            state = AbilityState.cooldown;

            await Task.Delay((int)(ability.cooldown * 1000));
        }

        state = AbilityState.ready;
    }
}
