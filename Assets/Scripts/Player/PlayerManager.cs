//Stats and heal/damage mechanics for players

using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerManager : Interactable, IHealth {
    [SerializeField] PlayerClass playerClass;
    [Header("Modifiable Stats")]
    public ModifiableStat maxHealth;
    public ModifiableStat moveSpeed;
    public ModifiableStat agroAmount;

    [Header("Health")]
    public float currentHealth;
    public bool AliveState = true;
    [Range(0, 1)]
    public float reviveHealthPercentage;
    public bool isInvincible = false;

    public int souls { get; private set; }
    public int kills { get; private set; }

    public PlayerInput input { get; private set; }

    [ReadOnly, SerializeField] bool currentlyInteractable;
    [ReadOnly, SerializeField] GameObject currentPrompt;

    GameObject deathLight;

    // Start is called before the first frame update
    private void Awake() {
        maxHealth.BaseValue = 100;
        maxHealth.AddModifiers(new(playerClass.stats.maxHealthMultiplier, StatModType.PercentAdd));
        currentHealth = maxHealth.Value;



        input = GetComponent<PlayerInput>();

        DontDestroyOnLoad(gameObject);
    }

    //Set up UI for players
    private void Start() {
        UpdatePlayerUIElement();

        SetColours();
    }

    private void OnEnable() {
        UpdatePlayerUIElement();

        GameManager.OnPause += Pause;
        GameManager.OnUnpause += UnPause;
    }

    private void OnDisable() {
        GameManager.OnPause -= Pause;
        GameManager.OnUnpause -= UnPause;
    }

    //Allows player to pause game
    public void Pause() {
        GetComponent<PlayerInput>().DeactivateInput();
        GetComponent<WeaponController>().CancelShooting();
    }

    //Allows player to unpause game
    private void UnPause() {
        GetComponent<PlayerInput>().ActivateInput();
    }

    //Damage to the player from enemies or objects
    public void Damage(HitData hitData, GameObject source) {
        if (!isInvincible) {
            currentHealth -= hitData.damage;

            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth.Value);

            if (currentHealth <= 0) {
                Kill();
            }
            UpdatePlayerUIElement();
        }
    }

    //Gives the player health
    public void Heal(float healAmount) {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth.Value);
        UpdatePlayerUIElement();
    }

    //Kills player on 0 health
    public void Kill() {
        if (AliveState == false)
            return;

        if (deathLight != null) Destroy(deathLight);

        deathLight = Instantiate(Resources.Load("Prefabs/DeathLight") as GameObject, transform.position, Quaternion.identity);

        AliveState = false;
        currentlyInteractable = true;
        GetComponent<PlayerAnimationController>().SetDead(true);
        GetComponent<PlayerInput>().DeactivateInput();
        GetComponent<WeaponController>().CancelShooting();
        GetComponent<WeaponController>().SetLineRendererActive(false);
        GetComponentInChildren<WeaponHolderController>(true).gameObject.SetActive(false);
        GameObject.Find("GamePlayManager").GetComponent<GameEventsManager>().CheckRemainingPlayers();
    }

    //Allows player to revive other player
    public void Revive(GameObject source) {
        if (AliveState == true)
            return;

        if (deathLight != null) Destroy(deathLight);

        AliveState = true;
        currentlyInteractable = false;
        GetComponent<PlayerAnimationController>().SetDead(false);
        GetComponent<WeaponController>().SetLineRendererActive(true);
        GetComponentInChildren<WeaponHolderController>(true).gameObject.SetActive(true);

        if (source != null)
            currentHealth = source.GetComponent<PlayerManager>().reviveHealthPercentage * maxHealth.Value;
        else currentHealth = maxHealth.Value;

        GetComponent<PlayerInput>().ActivateInput();
        UpdatePlayerUIElement();
    }

    //Sets off revive progress for player
    public IEnumerator ReviveCoroutine(GameObject source) {
        if (AliveState == false) {
            float reviveTime = 0;
            bool successRevive = true;

            if (currentPrompt != null)
                Destroy(currentPrompt);

            CircularProrgressBar bar = Instantiate(PromptUtil.CreateCircularProgressBar(gameObject, new(0, 60), 50), UIManager.Current.transform);
            currentPrompt = bar.gameObject;

            bar.Init(4, transform, CircularProrgressBar.CircularProgressType.Increase, source.GetComponent<PlayerManager>().PlayerClass.classColour);

            //Uninterrupted revival progress
            while (reviveTime < 4 && successRevive) {
                reviveTime += Time.deltaTime;
                if (Vector3.Distance(source.transform.position, transform.position) >= 3) {
                    successRevive = false;
                }
                yield return null;
            }

            if (successRevive) {
                Revive(source);
            }
            else {
                bar.Cancel();
                yield return null;
            }

            yield return null;
        }

        yield return null;
    }

    public override void Interact(GameObject source) {
        if (!currentlyInteractable)
            return;

        StartCoroutine(ReviveCoroutine(source));
    }

    public override void UnView() {
        if (currentPrompt != null)
            Destroy(currentPrompt);
    }

    public override void View(PlayerInput viewerInput) {
        if (currentlyInteractable == false || currentPrompt != null)
            return;

        currentPrompt = Instantiate(PromptUtil.CreateInteractionPrompt(gameObject, new(0, 50), viewerInput), UIManager.Current.transform);
    }

    public PlayerDetails GetPlayerDetails() {
        WeaponController wepCont = GetComponent<WeaponController>();

        return new(playerClass, maxHealth.Value, currentHealth, wepCont.ActiveGunSlot.gun, wepCont.NextGunSlot().gun, wepCont.ActiveGunSlot.currentMag, wepCont.ActiveGunSlot.currentAmmo /*,soulAmount*/);
    }

    public void UpdatePlayerUIElement() {
        if (UIManager.Current == null)
            return;

        UIManager.Current.UpdatePlayerInfo(input.playerIndex, this);
    }

    private void SetColours() {
        LineRenderer[] lineRenderers = GetComponentsInChildren<LineRenderer>();

        Color lineRendererStartColour = playerClass.classColour;
        Color lineRendererEndColour = playerClass.classColour;

        lineRendererStartColour.a = lineRenderers[0].startColor.a;
        lineRendererEndColour.a = lineRenderers[0].endColor.a;

        foreach (LineRenderer lineRenderer in lineRenderers) {
            lineRenderer.startColor = lineRendererStartColour;
            lineRenderer.endColor = lineRendererEndColour;
        }
    }

    public void ModifySouls(int soulAmount, SoulModification modificationType, bool shareSouls = false, float shareRatio = 0) {
        if (modificationType == SoulModification.add)
            souls += soulAmount;
        else
            souls -= soulAmount;

        souls = Mathf.Clamp(souls, 0, int.MaxValue);

        if (shareSouls) {
            GameManager.ShareSouls(gameObject, (int)(soulAmount * shareRatio));
        }

        UpdatePlayerUIElement();
    }

    public void AddKill() {
        AwardStatistic("Kills", 1);
        kills++;
    }

    public enum SoulModification {
        add,
        remove
    }

    public PlayerClass PlayerClass => playerClass;

    public void AwardStatistic(string statName, int increment) {
        if (!LocalMultiplayerManager.Instance) { return; }

        LocalMultiplayerManager.Instance.players[input.playerIndex].TrackedPlayerStats.IncrementStat(statName, increment);
    }

    [ContextMenu("RevivePlayer")]
    public void ContextMenuRevivePayer() {
        Revive(null);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerManager))]
[CanEditMultipleObjects]
public class PlayerManagerEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        PlayerManager player = (PlayerManager)target;

        //Adds a button to fill ammo to max
        if (GUILayout.Button("Damage Player")) {
            player.Damage(new(Vector3.zero, Vector3.zero, 10, DamageType.generic), null);
        }
    }
}
#endif


public struct PlayerDetails {
    public PlayerClass playerClass;

    public float maxHealth;
    public float health;

    public GunBase currentGun;
    public GunBase nextGun;
    public int currentAmmo;
    public int reserveAmmo;

    public PlayerDetails(PlayerClass _playerClass, float _maxHealth, float _health, GunBase _base, GunBase _nextGun, int _currentAmmo, int _reserveAmmo /*,int _collectedSouls*/) {
        playerClass = _playerClass;

        maxHealth = _maxHealth;
        health = _health;
        currentGun = _base;
        nextGun = _nextGun;
        currentAmmo = _currentAmmo;
        reserveAmmo = _reserveAmmo;
        //collectedSouls = _collectedSouls;
    }
}