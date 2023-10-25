using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RequiresCaster : MonoBehaviour
{
    private protected GameObject caster;

    public void Init(GameObject source) {
        caster = source;
    }
}
