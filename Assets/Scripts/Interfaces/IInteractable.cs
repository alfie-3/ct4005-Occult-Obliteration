//Interface for interactions

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public void Interact(GameObject source);
    public void View(PlayerInput viewerInput);
    public void UnView();

}
