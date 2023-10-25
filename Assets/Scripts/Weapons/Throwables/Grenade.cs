//Normal grenade

using System.Collections;
using UnityEngine;

public class Grenade : BaseGrenade {

    private void Awake() {
        StartCoroutine(FuseCountdown());
    }

    //Countdown until the grenade explodes
    IEnumerator FuseCountdown() {
        yield return new WaitForSeconds(fuseDuration);
        Detonate();
    }

    //Grenade explosion 
    public override void Detonate() {
        grenadeModel.SetActive(false);  
        explosion.SetActive(true);
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);

        //damage all entities within the explosion radius
        foreach (Collider collider in colliders) {
            if (collider.TryGetComponent(out IHealth healthInterface)) {
                HitData data = new(collider.transform.position, Vector3.zero, damage, DamageType.fire);
                healthInterface.Damage(data, caster);
            }
        }

        if (explosionClip != null)
            AudioSource.PlayClipAtPoint(explosionClip, transform.position, 0.5f);

        Destroy(gameObject, 1);
    }
}
