//Attempted acid trap for the trapper
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AcidTrap : MonoBehaviour
{
    [Space]
    [Header("Initial overlap checker")]
    [SerializeField] Collider _collider;
    [SerializeField] LayerMask _layerMask;
    [Space]
    [Header("Acid trap values")]
    [SerializeField] float trapActiveTime;

    //Lists
    List<Collider> affectedEntitys;
    List<float> defaultSpeeds;

    int tempIndex;
    int counter;

    private void Awake() {
        affectedEntitys = new List<Collider>();
        defaultSpeeds = new List<float>();
        StartCoroutine(AcidTime());
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.tag == "Enemy") {
            if (!affectedEntitys.Contains(collision)) {
                affectedEntitys.Add(collision);
                defaultSpeeds.Add(collision.GetComponent<NavMeshAgent>().speed);
                collision.GetComponent<NavMeshAgent>().speed *= 0.5f;
            }
        }
    }

    private void OnTriggerExit(Collider collision) {
        if (collision.gameObject.tag == "Enemy" && affectedEntitys.Contains(collision)) {
            tempIndex = affectedEntitys.IndexOf(collision);
            collision.GetComponent<NavMeshAgent>().speed = defaultSpeeds[tempIndex];
            affectedEntitys.RemoveAt(tempIndex);
            defaultSpeeds.RemoveAt(tempIndex);
        }
    }

    IEnumerator AcidTime() {
        yield return new WaitForSeconds(trapActiveTime);
        Destroy(gameObject);
    }

    private void OnDestroy() {
        counter = 0;
        foreach (Collider entity in affectedEntitys) {
            entity.GetComponent<NavMeshAgent>().speed = defaultSpeeds[counter];
            counter++;
        }
    }
}
