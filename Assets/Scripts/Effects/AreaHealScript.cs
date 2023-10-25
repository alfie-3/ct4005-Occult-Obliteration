using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.EventSystems.EventTrigger;

public class AreaHealScript : MonoBehaviour {

    [Space]
    [Header("Initial overlap checker")]
    [SerializeField] LayerMask _layerMask;
    [SerializeField] int healRadius;
    [Space]
    [Header("Burst Heal Values")]
    [SerializeField] float healAmount;
    [SerializeField] float healIntervalTime;
    [SerializeField] int healIterationAmount;

    List<GameObject> playersToHeal;

    private void Awake() {
        playersToHeal = new List<GameObject>();
        StartCoroutine(BurstHeal());
    }

    //Heals players in an area multiple times over time
    IEnumerator BurstHeal() {
        yield return new WaitForSeconds(0.1f);
        foreach (Collider entity in Physics.OverlapSphere(gameObject.transform.position, healRadius)) {
            if (entity.gameObject.tag == "Player") {
                playersToHeal.Add(entity.gameObject);
            }
        }
        for (int i = 0; i < healIterationAmount; i++) {
            foreach (GameObject player in playersToHeal) {
                if (player.TryGetComponent(out IHealth healthInt)) {
                    healthInt.Heal(healAmount);
                }
            }
            yield return new WaitForSeconds(healIntervalTime);
        }

        if(TryGetComponent(out VisualEffect vfxSystem)){
            vfxSystem.Stop();

            while (vfxSystem.aliveParticleCount > 0) {
                yield return null;
            }
        }
        
        Destroy(gameObject);
        yield return null;
    }
}
