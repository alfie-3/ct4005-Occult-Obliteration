//Holy grenade that is designed to heal players and damage enemies over time

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class HolyGrenadeObject : BaseGrenade {

    DamageEffect _damageEffect;
    HealEffect _healEffect;

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
        _damageEffect = GetComponent<DamageEffect>();
        _healEffect = GetComponent<HealEffect>();
        _damageEffect.Init(caster);
        _healEffect.Init(caster);
        StartCoroutine(_damageEffect.CheckArea(gameObject));
        StartCoroutine(_healEffect.CheckArea(gameObject));
        yield return new WaitForSeconds(Mathf.Max(_healEffect.iterationDelay * _healEffect.iterations, _damageEffect.iterations * _damageEffect.iterationDelay));
        GetComponent<VisualEffect>().Stop();
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}