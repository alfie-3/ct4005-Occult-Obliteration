//Base script for interactable items

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Interactable : MonoBehaviour, IInteractable
{
    [Header("Interactable Values")]
    public int priority;
    protected bool viewed;

    public abstract void Interact(GameObject source);
    public virtual void UnView() {
        viewed = false;
    }

    public virtual void View(PlayerInput viewerInput) {
        viewed = true;
    }
}
