using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeBehaviour : RequiresCaster
{
    [SerializeField] float rotationSpeed;
    [SerializeField] float travelSpeed;
    [SerializeField] int damage;
    Vector3 casterForward;
    
    private void Start() {
        casterForward = caster.transform.forward;
        gameObject.transform.localRotation = Quaternion.Euler(90, caster.transform.rotation.y, 0);
        StartCoroutine(KnifeLifetime());
    }

    private void Update() {
        transform.Rotate(new Vector3(0, 0, rotationSpeed) * Time.deltaTime); 
        transform.Translate(travelSpeed * Time.deltaTime * casterForward, Space.World);
    }

    private void OnTriggerEnter(Collider collider) {
        Debug.Log(collider.name);
        if (collider.gameObject.TryGetComponent(out IHealth healthInterface)&&collider.tag=="Enemy") {
            Debug.Log("HIHI");
            HitData data = new(collider.transform.position, Vector3.zero, damage, DamageType.fire);
            healthInterface.Damage(data, caster);
        }
        else if (collider.tag != "Player" && collider.tag != "Enemy") {
            StartCoroutine(KnifeStop());
        }
    }

    IEnumerator KnifeStop() {
        travelSpeed = 0; rotationSpeed = 0; damage = 0;
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    IEnumerator KnifeLifetime() {
        yield return new WaitForSeconds(7);
        Destroy(gameObject);
    }
}
