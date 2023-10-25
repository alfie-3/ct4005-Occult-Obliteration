using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteChargeCollider : MonoBehaviour
{
    public GameObject caster;

    private void Update() {
        foreach (Collider collider in Physics.OverlapSphere(caster.transform.position, 1.7f)) {
            if (collider.tag == "Enemy" && collider.GetComponent<EnemyController>().isConscious) {
                collider.gameObject.transform.GetComponent<EnemyController>().StunInit();
                collider.GetComponent<IHealth>().Damage(new(collider.ClosestPoint(caster.transform.position), Vector3.zero, 5.0f), caster);
            }
        }
    }
}
