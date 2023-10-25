using System;
using UnityEngine;
using UnityEngine.VFX;

public class WeaponHolderController : MonoBehaviour {
    private bool applyWeaponMotion = true;
    [Space]
    [SerializeField, Range(0, 1)] float swayPeriod = 2;
    [SerializeField, Range(0, 1)] float swayAmplitude;
    [Space]
    [SerializeField] float weaponRecoilRecoverySpeed = 1f;
    [Space]
    [SerializeField] Transform bulletSpawn;
    [Space]
    [SerializeField] Transform frontGrip;
    [SerializeField] Transform backGrip;
    [Space]
    [SerializeField] Vector3 rightArmHintPos;
    [SerializeField] Vector3 leftArmHintPos;

    Vector3 startPos = Vector2.zero;
    float recoilPosZ;
    VisualEffect muzzleFlash;

    private void Start() {
        startPos = transform.localPosition;
        transform.root.GetComponentInChildren<PlayerIKController>().SetHandGripPos(frontGrip, leftArmHintPos, backGrip, rightArmHintPos);
        transform.root.GetComponent<WeaponController>().bulletSpawnPoint = bulletSpawn;
        muzzleFlash = bulletSpawn.GetComponent<VisualEffect>();
    }

    void Update() {
        Vector3 weaponMovement = ApplySway();
        weaponMovement.z += CalculateRecoil();

        if (applyWeaponMotion)
            transform.localPosition = weaponMovement;
    }

    public void SetApplyWeaponMotion(bool value) {
        applyWeaponMotion = value;
        transform.localPosition = startPos;
    }

    private Vector3 ApplySway() {
        float distanceY = swayAmplitude * Mathf.Sin(Time.timeSinceLevelLoad / swayPeriod);
        float distanceX = swayAmplitude * Mathf.Sin(Time.timeSinceLevelLoad + 1 / (swayPeriod));

        Vector3 swayDir = startPos + new Vector3(distanceX, distanceY, 0);

        return swayDir;
    }

    public void ApplyRecoil(float recoilAmount) {
        recoilPosZ -= recoilAmount;
        recoilPosZ = Mathf.Clamp(recoilPosZ, -0.16f, 0);
    }

    public void PlayMuzzleFlash() {
        muzzleFlash.Play();
    }

    private float CalculateRecoil() {
        recoilPosZ += weaponRecoilRecoverySpeed * Time.deltaTime;

        recoilPosZ = Mathf.Clamp(recoilPosZ, -0.16f, 0);

        return recoilPosZ;
    }
}
