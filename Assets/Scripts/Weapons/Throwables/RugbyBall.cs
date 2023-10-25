using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RugbyBall : RequiresCaster
{
    [SerializeField] float throwSpeed;
    [SerializeField] float spinSpeed = 700f;
    [Space]
    [SerializeField] int maxBounced = 3;
    [SerializeField] float lifeSpan = 10;
    int currentBounced;
    [SerializeField] LayerMask bounceLayerMask;
    GameObject ballModel;
    [SerializeField] float hitDamage = 20;

    private void Start() {
        ballModel = GetComponentInChildren<MeshRenderer>().gameObject;
        Invoke(nameof(DestroyBall), lifeSpan);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Enemy")) {
           if (other.gameObject.TryGetComponent(out IHealth health)) {
                health.Damage(new(transform.position, Vector3.zero, hitDamage), caster);
           }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(throwSpeed * Time.deltaTime * transform.forward, Space.World);

        Spin();
        CheckForSurface();
    }

    //Spins the rugby ball
    private void Spin() {
        ballModel.transform.Rotate(new Vector3(spinSpeed, 0, 0) * Time.deltaTime);
    }

    private void CheckForSurface() {
        if (Physics.Raycast(transform.position, transform.forward * 1, out RaycastHit hit, 1, bounceLayerMask)) {

            Vector3 bounceDir = Vector3.Reflect(transform.forward, hit.normal);
            bounceDir.y = 0;
            transform.rotation = Quaternion.LookRotation(bounceDir);

            if (currentBounced == maxBounced) {
                Destroy(gameObject);
            }

            currentBounced++;
        }
    }

    private void DestroyBall() {
        Destroy(gameObject);
    }
}
