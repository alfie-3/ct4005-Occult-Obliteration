//Teleport player in movement direction

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Teleport Ability", menuName = "Abilities/Teleport Ability", order = 1)]
public class TeleportAbility : Ability {

    [SerializeField] GameObject particleSystem;
    GameObject particleSystem1;
    GameObject particleSystem2;
    PlayerControlScript _playerControlScript;
    Vector3 move;
    int distanceTeleport;
    [SerializeField] LayerMask layerMask;

    //Teleport to location
    public override bool Activate(GameObject caster) {
        PlayerControlScript controller = caster.GetComponent<PlayerControlScript>();
        particleSystem1 = Instantiate(particleSystem);
        particleSystem2 = Instantiate(particleSystem);
        distanceTeleport = 8;
        _playerControlScript = caster.GetComponent<PlayerControlScript>();
        base.Activate(caster);
        move = new Vector3(_playerControlScript.move.x, 0, _playerControlScript.move.y);
        particleSystem1.transform.position = caster.transform.position;
        particleSystem1.GetComponent<ParticleSystem>().Play();
        if (move.normalized != Vector3.zero) {
            RaycastHit hit;
            if (Physics.Raycast(caster.transform.position, move.normalized, out hit, distanceTeleport,layerMask)) {
                controller.TeleportPlayer(hit.point);
            }
            else {
                controller.TeleportPlayer(caster.transform.position + ((move.normalized) * distanceTeleport));
            }
        }
        particleSystem2.transform.position = caster.transform.position;
        particleSystem2.GetComponent<ParticleSystem>().Play();
        _playerControlScript.isDashing = false;

        return true;
    }

    //End teleport ability
    public override void Deactivate(GameObject caster) {
        Destroy(particleSystem1);
        Destroy(particleSystem2);
    }
}

