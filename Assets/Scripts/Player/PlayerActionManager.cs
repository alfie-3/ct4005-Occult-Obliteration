//Script for player interaction

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActionManager : MonoBehaviour {
    [Header("Interaction")]
    [SerializeField] Vector3 interactionCheckBoxLocation;
    [SerializeField] Vector3 interactionCheckBoxSize;
    [Space]
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] bool showInteractionBoxGizmo = true;

    [SerializeField] private Collider[] interactionColliders = new Collider[10];

    PlayerInput input;
    [ReadOnly, SerializeField] Interactable currentInteractable;

    private void Awake() {
        input = GetComponent<PlayerInput>();
        input.actions["Interact"].performed += Interact;
    }

    private void FixedUpdate() {
        CheckInteraction();
    }

    //Check box area in front of player for any interactions
    public void CheckInteraction() {
        Vector3 colliderPos = transform.TransformPoint(interactionCheckBoxLocation);
        int colliderLength = Physics.OverlapBoxNonAlloc(colliderPos, interactionCheckBoxSize / 2, interactionColliders, transform.rotation, interactableLayerMask);

        if (colliderLength == 1) {
            if (currentInteractable != null) {
                currentInteractable.UnView();
                currentInteractable = null;
            }

            return;
        }

        Interactable pickedCollider = interactionColliders[0].GetComponent<Interactable>();

        for (int i = 0; i < colliderLength; i++) {
            if (pickedCollider.priority <= interactionColliders[i].GetComponent<Interactable>().priority) {
                if (interactionColliders[i].gameObject != gameObject)
                    pickedCollider = interactionColliders[i].GetComponent<Interactable>();
            }
        }

        if (currentInteractable != null) {
            if (currentInteractable != pickedCollider)
                currentInteractable.UnView();
        }

        if (pickedCollider.gameObject != gameObject) {
            currentInteractable = pickedCollider;
        }
        else {
            currentInteractable = null;
        }

        View();
    }

    public void View() {
        if (currentInteractable == null || currentInteractable == gameObject)
            return;

        currentInteractable.View(input);
    }

    //Interact with object within player box that has most priority
    public void Interact(InputAction.CallbackContext context) {
        if (currentInteractable == null)
            return;

        if (context.performed) {
            currentInteractable.Interact(gameObject);
            GetComponent<WeaponController>().CancelShooting();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.matrix = transform.localToWorldMatrix;

        if (showInteractionBoxGizmo) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(interactionCheckBoxLocation, interactionCheckBoxSize);

            if (currentInteractable != null) {
                Gizmos.matrix = Matrix4x4.identity;

                Gizmos.DrawLine(transform.position, currentInteractable.transform.position);
            }
        }
    }
}


