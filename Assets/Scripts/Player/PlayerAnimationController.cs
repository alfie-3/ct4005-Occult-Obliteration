using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {
    Animator anim;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    public void SetAnimMoveVector(Vector2 _moveVector) {

        Vector3 move = _moveVector.y * Vector3.forward + _moveVector.x * Vector3.right;

        Vector3 localMove = transform.InverseTransformDirection(move);

        anim.SetFloat("MoveX", localMove.x, 0.1f, Time.deltaTime);
        anim.SetFloat("MoveY", localMove.z, 0.1f, Time.deltaTime);
    }

    public IEnumerator ReloadCoroutine(float reloadSpeed) {
        WeaponHolderController weaponHolder = GetComponentInChildren<WeaponHolderController>();

        anim.SetFloat("ReloadSpeed", 1 / reloadSpeed);
        weaponHolder.SetApplyWeaponMotion(false);
        anim.CrossFade("Reload", 0.03f, 3);
        yield return new WaitForSeconds(reloadSpeed);
        weaponHolder.SetApplyWeaponMotion(true);
    }

    public void SetDead(bool dead) {
        if (dead) {
            anim.SetBool("Dead", true);
            GetComponentInChildren<PlayerIKController>().SetHandIKWeight(0, 0);
        }
        else {
            anim.SetBool("Dead", false);
            GetComponentInChildren<PlayerIKController>().SetHandIKWeight(1, 1);
        }
    }
}
