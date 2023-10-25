using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TrapperTrap : BaseGrenade {


    SlowEffect _slowEffect;

    bool hasExploded = false;

    private void Awake() {
        GetComponent<VisualEffect>().Stop();
    }

    //Grenade smashes on contact with anything other than a player
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag != "Player" && !hasExploded) {
            hasExploded = true;
            AudioSource.PlayClipAtPoint(explosionClip, transform.position, 1f);
            StartCoroutine(Explosion());
        }
    }

    //Grenade effects when exploding
    IEnumerator Explosion() {
        //Visual effect starts at grenade to show area of effect
        GetComponent<VisualEffect>().Play();
        GetComponent<VisualEffect>().transform.position = gameObject.transform.position;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        //HolyGrenade entity effects
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        _slowEffect = GetComponent<SlowEffect>();
        StartCoroutine(_slowEffect.CheckArea(gameObject));
        yield return new WaitForSeconds(0.15f);
        GetComponent<VisualEffect>().Stop();
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
