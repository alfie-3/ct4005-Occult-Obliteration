//Attached to players to activate their abilities on key press or hold

using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityController : MonoBehaviour {

    [SerializeField] AbilitySlot primaryAbility;
    [SerializeField] AbilitySlot secondaryAbility;
    [Space]
    [SerializeField] AbilitySlot dashAbility;
    bool primaryButtonHeld, secondaryButtonHeld, dashButtonHeld;

    //Set inputs when controller/keyboard is enabled to
    private void OnEnable() {
        PlayerInput input = GetComponent<PlayerInput>();
        input.actions["Primary Ability"].performed += ActivatePrimaryAbility;
        input.actions["Primary Ability"].canceled += ActivatePrimaryAbility;
        input.actions["Secondary Ability"].performed += ActivateSecondaryAbility;
        input.actions["Secondary Ability"].canceled += ActivateSecondaryAbility;
        input.actions["Dash Ability"].performed += ActivateDashAbility;
        input.actions["Dash Ability"].canceled += ActivateDashAbility;
    }

    //removes inputs when controller/keyboard is disabled
    private void OnDisable() {
        PlayerInput input = GetComponent<PlayerInput>();
        input.actions["Primary Ability"].performed -= ActivatePrimaryAbility;
        input.actions["Primary Ability"].canceled -= ActivatePrimaryAbility;
        input.actions["Secondary Ability"].performed -= ActivateSecondaryAbility;
        input.actions["Secondary Ability"].canceled -= ActivateSecondaryAbility;
        input.actions["Dash Ability"].performed -= ActivateDashAbility;
        input.actions["Dash Ability"].canceled -= ActivateDashAbility;
    }

    //Activate button held abilities
    private void LateUpdate() {
        if (primaryButtonHeld)
            primaryAbility.ButtonHeld(gameObject);


        if (secondaryButtonHeld)
            secondaryAbility.ButtonHeld(gameObject);

        if (dashButtonHeld)
            dashAbility.ButtonHeld(gameObject);
    }

    //Activates first ability if they haven't been performed
    public void ActivatePrimaryAbility(InputAction.CallbackContext context) {

        if (primaryAbility != null && context.performed) {
            primaryButtonHeld = true;
        }

        if (primaryAbility != null && context.canceled) {
            primaryAbility.ActivateAbility(gameObject);
            primaryButtonHeld = false;
        }
    }

    //Activates second ability if they haven't been performed
    public void ActivateSecondaryAbility(InputAction.CallbackContext context) {
        if (secondaryAbility != null && context.performed) {
            secondaryButtonHeld = true;
        }

        if (secondaryAbility != null && context.canceled) {
            secondaryAbility.ActivateAbility(gameObject);
            secondaryButtonHeld = false;
        }
    }

    //Activates dash ability if they haven't been performed
    public void ActivateDashAbility(InputAction.CallbackContext context) {
        if (dashAbility != null && context.performed) {
            dashButtonHeld = true;
        }

        if (dashAbility != null && context.canceled) {
            dashAbility.ActivateAbility(gameObject);
            dashButtonHeld = false;
        }
    }
}
