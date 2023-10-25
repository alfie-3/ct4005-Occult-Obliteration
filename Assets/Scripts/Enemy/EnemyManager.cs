//Contains important information for enemies

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class EnemyManager : MonoBehaviour, IHealth {
    //Wave list managing
    WaveManager _waveManager;
    //LocalMultiplayerManager _localMultiplayerManager;
    
    [SerializeField] EnemyStats stats;

    [SerializeField] float health;
    public float baseDamage;
    public int soulAmount;

    public EnemyHealth _enemyHealth;
    [SerializeField] GameObject healthBar;
    [SerializeField] Vector2 offsetSetting;
    [SerializeField] List<GameObject> itemDrops;
    [SerializeField, Range(0.01f,1)] float itemSpawnChance;

    //Sets initial values
    private void Start() {
        EnemyHealth health = EnemyHealth.CreateHealthBar(gameObject);//, new Vector2(0, 100));
        health.enemy = gameObject;
        _enemyHealth = Instantiate(health, UIManager.Current.transform);
        _enemyHealth.GetComponent<EnemyHealth>().Init(offsetSetting);
        _enemyHealth.gameObject.SetActive(false);
    }
    
    void OnEnable() {
        health = stats.maxHealth;
    }

    //Resets enemy health based on player multiplier
    public void Init(float multiplier) {
        stats.maxHealth *= multiplier;
        health = stats.maxHealth;
    }

    //Summoned when enemy takes damage
    public void Damage(HitData hitData, GameObject source) {
        if (_enemyHealth.isActiveAndEnabled == false)
            _enemyHealth.gameObject.SetActive(true);

        Debug.Log(hitData.damage);

        health -= hitData.damage;

        if (health <= 0) {
            Kill(source, hitData.damage);
        }

        if (source.TryGetComponent(out PlayerManager manager)) {
            manager.AwardStatistic("Damage Dealt", (int)hitData.damage);
        }

        _enemyHealth.UpdateHealthBar(stats.maxHealth, health);
    }

    //Summoned when enemies are healed
    public void Heal(float healAmount) {
        float healthPlusHeal = health + healAmount;
        Mathf.Clamp(healthPlusHeal, 0, stats.maxHealth);
        _enemyHealth.UpdateHealthBar(stats.maxHealth, health);
    }

    //Delete enemy, chance to drop loot and gives player soul on kill
    public void Kill(GameObject source, float damage = 0) {
        if (_waveManager != null)
            _waveManager.enemyList.Remove(gameObject);

        if (source.TryGetComponent(out PlayerManager manager)) {
            manager.ModifySouls(soulAmount, PlayerManager.SoulModification.add, true, 0.25f);
            manager.AddKill();
        }

        int pickupInt = UnityEngine.Random.Range(0, Mathf.CeilToInt((itemDrops.Count) * (1 / itemSpawnChance)));
        if (pickupInt < itemDrops.Count) {
            Instantiate(itemDrops[pickupInt], gameObject.transform.position, Quaternion.identity);
        }

        Destroy(_enemyHealth.gameObject);
        Destroy(gameObject);
    }
}

[System.Serializable]
public class EnemyStats {
    public float maxHealth = 10;

}