using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Brute Charge Ability", menuName = "Abilities/Brute Charge", order = 1)]
public class BruteCharge : Ability
{
    PlayerControlScript _playerControlScript;
    PlayerManager _playerManager;
    Vector3 move;
    GameObject collider;

    [SerializeField] float damage;
    [SerializeField] GameObject bruteChargeCollider;
    [SerializeField] GameObject effectPrefab;
    [SerializeField] float effectDuration = 1.5f;
    GameObject activeEffectObject;

    public override bool Activate(GameObject caster) {
        _playerManager = caster.GetComponent<PlayerManager>();
        _playerControlScript = caster.GetComponent<PlayerControlScript>();
        collider = Instantiate(bruteChargeCollider, caster.transform.position, Quaternion.identity, caster.transform);
        collider.GetComponent<BruteChargeCollider>().caster = caster;
        base.Activate(caster);
        move = _playerControlScript.move;
        if (move.normalized != Vector3.zero) {
            Vector3 effectSpawnDir = new(_playerControlScript.move.x, 0, _playerControlScript.move.y);

            if (effectPrefab) {
                activeEffectObject = Instantiate(effectPrefab, caster.transform.position + -effectSpawnDir.normalized * 5, Quaternion.LookRotation(-effectSpawnDir, Vector3.up));
                activeEffectObject.GetComponent<TrailFollow>().Follow(caster.transform, Vector3.zero);
                activeEffectObject.
                GetComponent<TrailFollow>().DestroyTrail(effectDuration);
            }
            _playerControlScript.isDashing = true;
            _playerManager.isInvincible = true;
            caster.transform.forward = new Vector3(move.x,0,move.y);
            _playerControlScript.move = (move.normalized)*3;
            return true;
        }
        return false;
    }

    public override void Deactivate(GameObject caster) {
        base.Deactivate(caster);
        Destroy(collider);
        _playerControlScript.isDashing = false;
        _playerManager.isInvincible = false;

    }
}
