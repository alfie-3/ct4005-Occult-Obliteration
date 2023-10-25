using System.Collections;
using System.Collections.Generic;
//Base script to control how abilities are used
using UnityEngine;

public class Ability : ScriptableObject
{
    public new string name;
    [TextArea] public string decription;
    public float cooldown;
    public float duration;

    public virtual void ButtonHeld(GameObject caster) { }

    public virtual bool Activate (GameObject caster) { return false; }

    public virtual void Deactivate(GameObject caster) { }
}
