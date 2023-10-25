//Base class & info for guns

using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class GunBase : ScriptableObject {
    public GunData gunData;
    [Space]
    [SerializeReference] public GunStats stats;
    [Space]
    public GunAudioAsset gunAudio;
    public GunTrailAsset gunTrail;

    public GunType gunType = GunType.Rifle;

    public GunBase() {
        stats = new GunStats();
    }

    public abstract void Fire(WeaponController controller);
}

[System.Serializable]
public class GunData {
    public GameObject gunPrefab;
    [Header("Gun Info")]
    public Sprite gunIcon;
    public string gunName = "Gun Name";
    public int price = 100;
    public WeaponRarity weaponRarity = WeaponRarity.Common;
}

[System.Serializable]
public class GunStats {
    public FiringType firingType;

    [Header("Ammo")]
    public int maxMagSize = 10;
    public int maxAmmo = 100;

    [Header("Damage")]
    public float damage = 1;
    public DamageType damageType;
    public int pierceAmount = 1;

    [Header("Spread & Range")] //Min spread is x, max spread is y
    public float range = 40;
    [Space]
    [MinMaxSlider(1, 180, FlexibleFields = true)]
    public Vector2 spread = new(10, 20);
    public float spreadIncreaseAmount = 1;
    public float spreadDecreaseSpeed = 5f;

    [Header("Speed")]
    public float reloadSpeed = 0.5f;
    public float firingRate = 0.5f;
}

public enum GunType {
    Pistol,
    SMG,
    Rifle,
    Shotgun
}

