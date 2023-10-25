//Base script for all objects that can be collected by walking over

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBase : MonoBehaviour
{
    private void Awake() {
        StartCoroutine(SpawnTimer());
    }

    bool hasPlayerCollided=false;

    //Collect object on collision
    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.tag == "Player" && !hasPlayerCollided) {
            hasPlayerCollided = true;
            PickupPower(collision.gameObject);
        }
    }

    //Overwritten function for when the object is picked up
    public virtual void PickupPower(GameObject entity) { }

    //If the item is not picked up it will self destruct
    IEnumerator SpawnTimer() {
        yield return new WaitForSeconds(20);
        Destroy(gameObject);
    }
}
