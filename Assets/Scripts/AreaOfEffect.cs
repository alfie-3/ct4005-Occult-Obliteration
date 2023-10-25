//Creates area check to give effects on certain entities

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffect : RequiresCaster {

    [Header("Check area")]
    public float radius;
    [HideInInspector] Vector3 sphereSweep = Vector3.up;
    Collider[] results = new Collider[100];
    public LayerMask _layerMask;
    [Space]
    [Header("Effect values")]
    public int valueAffectAmountPerIteration;
    public int iterations;
    public float iterationDelay;

    //Checks area within a radius for entities
    public IEnumerator CheckArea(GameObject positionFinder) {

        for (int i = 0; i < iterations; i++) {
            int castNumber = Physics.OverlapSphereNonAlloc(positionFinder.transform.position, radius, results,_layerMask);
            for (int j = 0; j < castNumber; j++) {
                AffectEntity(results[j].gameObject, caster);
            }
            yield return new WaitForSeconds(iterationDelay);
        }
    }

    //Entity function to be overridden
    public virtual void AffectEntity(GameObject entity, GameObject source) { }

}
