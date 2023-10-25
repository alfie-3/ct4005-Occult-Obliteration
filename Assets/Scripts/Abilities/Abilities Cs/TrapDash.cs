using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trap Dash Ability", menuName = "Abilities/TrapDash Ability", order = 1)]
public class TrapDash : Ability
{
    PlayerControlScript _playerControlScript;
    Vector3 move;

    [SerializeField] GameObject effectPrefab;
    [SerializeField] GameObject locationMarkerPrefab;
    [SerializeField] AudioClip soundEffect;
    [Space]
    [SerializeField] float effectDuration = 1.5f;
    [SerializeField] GameObject explodyMine;
    GameObject activeEffectObject, activeMarkerObject;
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

    public override bool Activate(GameObject caster) {
        _playerControlScript = caster.GetComponent<PlayerControlScript>();
        base.Activate(caster);
        move = _playerControlScript.move;

        if (activeMarkerObject != null)
            Destroy(activeMarkerObject);

        if (move.normalized != Vector3.zero) {
            Instantiate(explodyMine, caster.transform.position, Quaternion.identity);
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

    public override void Deactivate(GameObject caster) {
        base.Deactivate(caster);

        _playerControlScript.isDashing = false;

    }
}
