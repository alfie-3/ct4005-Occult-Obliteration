//Dash that heals surrounding players

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Support Dash Ability", menuName = "Abilities/Support Dash", order = 1)]
public class SupportDash : Ability {
    PlayerControlScript _playerControlScript;
    Vector3 move;

    [SerializeField] GameObject effectPrefab;
    [SerializeField] GameObject locationMarkerPrefab;
    [SerializeField] AudioClip soundEffect;
    [Space]
    [SerializeField] float effectDuration = 1.5f;
    GameObject activeEffectObject, activeMarkerObject;

    [SerializeField] GameObject healField;

    //Shows dash location
    public override void ButtonHeld(GameObject caster) {
        base.ButtonHeld(caster);

        if (!_playerControlScript)
            _playerControlScript = caster.GetComponent<PlayerControlScript>();

        Vector3 effectSpawnDir = new(_playerControlScript.move.x, 0, _playerControlScript.move.y);

        if (activeMarkerObject == null && locationMarkerPrefab != null) {
            activeMarkerObject = Instantiate(locationMarkerPrefab, (caster.transform.position + effectSpawnDir.normalized * 5) - new Vector3(0, 1, 0), Quaternion.Euler(90, 0, 0));
        }

        activeMarkerObject.SetActive(effectSpawnDir != Vector3.zero);

        activeMarkerObject.transform.position = (caster.transform.position + effectSpawnDir.normalized * 5) - new Vector3(0, 1, 0);
    }

    //Dashes on button release
    public override bool Activate(GameObject caster) {
        _playerControlScript = caster.GetComponent<PlayerControlScript>();
        Instantiate(healField, caster.transform.position, Quaternion.identity);
        base.Activate(caster);
        move = _playerControlScript.move;

        if (activeMarkerObject != null)
            Destroy(activeMarkerObject);

        //If player is moving, dash in that direction
        if (move.normalized != Vector3.zero) {
            Vector3 effectSpawnDir = new(_playerControlScript.move.x, 0, _playerControlScript.move.y);

            if (effectPrefab) {
                activeEffectObject = Instantiate(effectPrefab, caster.transform.position + -effectSpawnDir.normalized * 5, Quaternion.LookRotation(-effectSpawnDir, Vector3.up));
                activeEffectObject.GetComponent<TrailFollow>().Follow(caster.transform, Vector3.zero);
                activeEffectObject.
                GetComponent<TrailFollow>().DestroyTrail(effectDuration);
            }

            caster.GetComponent<AudioSource>().PlayOneShot(soundEffect);


            _playerControlScript.isDashing = true;
            _playerControlScript.move = (move.normalized) * 5;

            return true;
        }

        return false;
    }    

    //End dash ability
    public override void Deactivate(GameObject caster) {
        base.Deactivate(caster);
        Instantiate(healField, caster.transform.position, Quaternion.identity);
        _playerControlScript.isDashing = false;

    }

}