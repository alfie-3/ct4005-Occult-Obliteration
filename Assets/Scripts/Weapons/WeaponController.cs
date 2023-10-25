//Controls how the players can fire the guns

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class WeaponController : MonoBehaviour {
    [SerializeField] List<GunSlot> gunSlots;
    [SerializeField] int activeGunSlotInt;
    [SerializeField] LayerMask shotLayerMask;
    [Space]
    public Transform bulletSpawnPoint;
    [Header("Spread")]
    [SerializeField] GameObject lineRendererHolder;
    private LineRenderer[] lineRenderers;
    [Space]
    [ReadOnly, SerializeField] private float currentSpread;
    [SerializeField] AnimationCurve spreadCurve;
    GameObject currentWeaponObject;

    CancellationTokenSource reloadCancelToken;

    private enum FiringStatus {
        ready,
        firing,
        reloading
    }

    [ReadOnly, SerializeField] private FiringStatus firingStatus = FiringStatus.ready;

    [ReadOnly, SerializeField] bool shootButtonHeld;

    PlayerManager playerManager;
    AudioSource audioSource;

    private void Awake() {
        lineRenderers = GetComponentsInChildren<LineRenderer>();
        playerManager = GetComponent<PlayerManager>();

        UpdateSpread(weaponSwapped: true);

        foreach (GunSlot slot in gunSlots) {
            if (slot.gun != null)
                slot.InitPool();
        }
    }

    private void Start() {
        UpdateWeaponObject();
    }

    private void Update() {
        UpdateSpread();
    }

    private void OnEnable() {
        TryGetComponent<PlayerInput>(out PlayerInput input);
        input.actions["Shoot"].canceled += ShootInput;
        input.actions["Shoot"].performed += ShootInput;

        input.actions["Reload"].performed += ReloadInput;
        input.actions["Swap Weapon"].performed += SwapWeapons;

        audioSource = GetComponent<AudioSource>();
    }
    private void OnDisable() {
        TryGetComponent<PlayerInput>(out PlayerInput input);
        input.actions["Shoot"].canceled -= ShootInput;
        input.actions["Shoot"].performed -= ShootInput;

        input.actions["Reload"].performed -= ReloadInput;
        input.actions["Swap Weapon"].performed -= SwapWeapons;

        foreach (GunSlot slot in gunSlots) {
            if (slot.gun != null)
                slot.FlushPool();
        }

        StopAllCoroutines();
    }

    //Shoots bullets if it's able to
    protected async void Shoot() {
        if (firingStatus != FiringStatus.ready)
            return;

        //Reload if mag is empty
        if (ActiveGunSlot.currentMag <= 0) {
            if (ActiveGunSlot.currentAmmo == 0)
                return;

            await Reload();

            await Task.Yield();
            return;
        }

        //Switches the way the gun fires by the firing type.
        switch (ActiveGunSlot.gun.stats.firingType) {

            case FiringType.SelectFire:
                firingStatus = FiringStatus.firing;
                Fire();
                DecrementAmmo();
                await WeaponCooldown();

                if (firingStatus != FiringStatus.reloading)
                    firingStatus = FiringStatus.ready;
                break;

            case FiringType.Automatic:
                AutoFire();
                break;

            case FiringType.BurstFire:
                BurstFire();
                break;
        }

        await Task.Yield();
    }

    //Fire once
    private void Fire() {
        ActiveGunSlot.gun.Fire(this);
        WeaponHolderController weaponHolderController = GetComponentInChildren<WeaponHolderController>();

        audioSource.PlayOneShot(ActiveGunSlot.gun.gunAudio.fireClip);
        weaponHolderController.ApplyRecoil(2);
        weaponHolderController.PlayMuzzleFlash();
        AddSpread(ActiveGunSlot.gun.stats.spreadIncreaseAmount);
    }

    //Fire continuously
    protected async void AutoFire() {

        while (shootButtonHeld) {
            if (!shootButtonHeld || firingStatus != FiringStatus.ready)
                break;

            if (ActiveGunSlot.currentMag <= 0) {
                await Reload();
                break;
            }

            firingStatus = FiringStatus.firing;
            Fire();
            DecrementAmmo();
            await WeaponCooldown();

            if (firingStatus != FiringStatus.reloading)
                firingStatus = FiringStatus.ready;
        }
    }

    //Fire 3 shots in a burst
    protected async void BurstFire() {
        int currentShots = 3;
        firingStatus = FiringStatus.firing;

        while (currentShots != 0) {

            if (ActiveGunSlot.currentMag == 0) {
                await Reload();
                break;
            }

            currentShots--;
            Fire();
            DecrementAmmo();
            await WeaponCooldown();
        }

        await WeaponCooldown();

        if (firingStatus != FiringStatus.reloading)
            firingStatus = FiringStatus.ready;
    }

    //Delay between weapons attack
    protected async Task WeaponCooldown() => await Task.Delay((int)(ActiveGunSlot.gun.stats.firingRate * 1000));

    //Reload weapons magazine
    protected async Task Reload() {
        if (firingStatus == FiringStatus.reloading || ActiveGunSlot.currentMag == ActiveGunSlot.gun.stats.maxMagSize || ActiveGunSlot.currentAmmo == 0)
            return;

        reloadCancelToken = new();
        CircularProrgressBar circularProrgressBar = null;
        Coroutine reloadAnimCoroutine = null;

        firingStatus = FiringStatus.reloading;
        reloadAnimCoroutine = StartCoroutine(GetComponent<PlayerAnimationController>().ReloadCoroutine(ActiveGunSlot.gun.stats.reloadSpeed));
        audioSource.PlayOneShot(ActiveGunSlot.gun.gunAudio.reloadClip);

        //Waits reload delay time before its completed.
        circularProrgressBar = Instantiate(PromptUtil.CreateCircularProgressBar(gameObject, new(0, 60), 50), UIManager.Current.transform);
        circularProrgressBar.Init(ActiveGunSlot.gun.stats.reloadSpeed, transform, CircularProrgressBar.CircularProgressType.Decrease, GetComponent<PlayerManager>().PlayerClass.classColour);
        try {
            await Task.Delay((int)(ActiveGunSlot.gun.stats.reloadSpeed * 1000), reloadCancelToken.Token); ;
        }
        catch {
            PreventReload(circularProrgressBar, reloadAnimCoroutine);
        }
        finally {
            reloadCancelToken.Dispose();
            reloadCancelToken = null;
        }
        //Math for transfering ammo from reserves to magazine.
        if (ActiveGunSlot.currentAmmo == 0) {
            firingStatus = FiringStatus.ready;
            return;
        }

        if (ActiveGunSlot.currentAmmo - (ActiveGunSlot.gun.stats.maxMagSize - ActiveGunSlot.currentMag) >= 0) {
            ActiveGunSlot.currentAmmo -= (ActiveGunSlot.gun.stats.maxMagSize - ActiveGunSlot.currentMag);
            ActiveGunSlot.currentMag = ActiveGunSlot.gun.stats.maxMagSize;
        }
        else {
            ActiveGunSlot.currentMag += ActiveGunSlot.currentAmmo;
            ActiveGunSlot.currentAmmo = 0;
        }

        firingStatus = FiringStatus.ready;
        playerManager.UpdatePlayerUIElement();
    }

    public void AddAmmo(int ammoAdded) {
        ActiveGunSlot.currentAmmo += ammoAdded;
        ActiveGunSlot.currentAmmo = Mathf.Clamp(ActiveGunSlot.currentAmmo, 0, ActiveGunSlot.gun.stats.maxAmmo);
        playerManager.UpdatePlayerUIElement();
    }

    //Stops player reload when needed
    protected void PreventReload(CircularProrgressBar circularProrgressBar, Coroutine reloadAnimCoroutine) {
        firingStatus = FiringStatus.ready;
        playerManager.UpdatePlayerUIElement();

        if (circularProrgressBar != null) {
            StopCoroutine(reloadAnimCoroutine);
            Destroy(circularProrgressBar.gameObject);
        }
    }
    protected void CancelReload() {
        reloadCancelToken.Cancel();
    }

    //Deplete ammo when guns are shot
    protected void DecrementAmmo() {
        ActiveGunSlot.currentMag--;
        playerManager.UpdatePlayerUIElement();
    }

    //Check if entities get hit by guns
    public void FireRaycast(Vector3 direction) {
        GunBase gun = ActiveGunSlot.gun;

        //Fires raycast and sorts order of array based on the distance of the target from the player
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction * 40, ActiveGunSlot.gun.stats.range, shotLayerMask);

        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        if (hits.Length == 0) {
            StartCoroutine(ActiveGunSlot.gun.gunTrail.PlayTrail(bulletSpawnPoint.position, transform.position + (direction * ActiveGunSlot.gun.stats.range), new RaycastHit(), ActiveGunSlot.trailPool));
            return;
        }

        int currentPiereced = 0;
        RaycastHit lastHit = new RaycastHit();

        for (int i = 0; i < hits.Length; i++) {
            //If the raycast hits an object with the ground layer the raycast will not reach any enemies behind the obstruction.
            if (hits[i].transform.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                StartCoroutine(ActiveGunSlot.gun.gunTrail.PlayTrail(bulletSpawnPoint.position, hits[i].point, hits[i], ActiveGunSlot.trailPool));
                return;
            }

            //When the raycast hits one enemy, the current pierce increases, once it reaches its max, no more enemies in the array will take damage
            if (hits[i].transform.gameObject.layer == LayerMask.NameToLayer("Enemy")) {

                HitData data = new(hits[i].point, hits[i].normal, ActiveGunSlot.gun.stats.damage, ActiveGunSlot.gun.stats.damageType);

                hits[i].transform.GetComponent<IHealth>().Damage(data, gameObject);

                StartCoroutine(ActiveGunSlot.gun.gunTrail.enemyHitEffect.PlayEffect(hits[i].point, hits[i].normal));

                lastHit = hits[i];
                currentPiereced++;
            }

            if (currentPiereced == ActiveGunSlot.gun.stats.pierceAmount)
                break;
        }

        if (currentPiereced == ActiveGunSlot.gun.stats.pierceAmount) {
            StartCoroutine(ActiveGunSlot.gun.gunTrail.PlayTrail(bulletSpawnPoint.position, lastHit.point, lastHit, ActiveGunSlot.trailPool));
        }
        else if (currentPiereced != ActiveGunSlot.gun.stats.pierceAmount) {
            StartCoroutine(ActiveGunSlot.gun.gunTrail.PlayTrail(bulletSpawnPoint.position, transform.position + (direction * ActiveGunSlot.gun.stats.range), new RaycastHit(), ActiveGunSlot.trailPool));
        }
    }

    //Lerps the spread to the default resting spread, if the weapon is swapped this method can be used to update the spread to the current weapon.
    public void UpdateSpread(bool weaponSwapped = false) {
        if (ActiveGunSlot.gun == null)
            return;

        float spread = currentSpread;

        if (weaponSwapped) {
            spread = ActiveGunSlot.gun.stats.spread.x;
            currentSpread = spread;
            return;
        }

        float defaultSpread = ActiveGunSlot.gun.stats.spread.x;
        float maxSpread = ActiveGunSlot.gun.stats.spread.y;

        float currentEval = (spread - defaultSpread) / (maxSpread - defaultSpread);

        //If the current spread is close enough to the default spread, its rounded to it.
        if (currentSpread > defaultSpread + 0.1) {
            spread -= (ActiveGunSlot.gun.stats.spreadDecreaseSpeed * spreadCurve.Evaluate(currentEval)) * Time.deltaTime;
        }
        else {
            spread = defaultSpread;
        }

        spread = Mathf.Clamp(spread, defaultSpread, maxSpread);

        currentSpread = spread;
        UpdateLineRenderers(currentSpread);
    }

    //Adds spread to the current spread
    public void AddSpread(float spreadAmmount) => currentSpread += spreadAmmount;

    public float GetSpread() => currentSpread;

    //Updates the line renderers petruding from the player character that indicate the current spread
    private void UpdateLineRenderers(float spread) {
        spread = (ActiveGunSlot.gun.gunType == GunType.Shotgun) ? spread / 2 : spread;

        Vector3 lineRendererPos = Quaternion.AngleAxis(spread, transform.up) * Vector3.forward;
        lineRenderers[0].SetPosition(0, lineRendererPos * 0.5f);
        lineRenderers[0].SetPosition(1, lineRendererPos * ActiveGunSlot.gun.stats.range / 4);

        lineRendererPos = Quaternion.AngleAxis(-spread, transform.up) * Vector3.forward;
        lineRenderers[1].SetPosition(0, lineRendererPos * 0.5f);
        lineRenderers[1].SetPosition(1, lineRendererPos * ActiveGunSlot.gun.stats.range / 4);
    }

    public void SetLineRendererActive(bool value) {
        foreach (LineRenderer lineRenderer in lineRenderers) {
            lineRenderer.gameObject.SetActive(value);
        }
    }

    //Alternate between 2 weapons that can be held at a time
    public void SwapWeapons(InputAction.CallbackContext context) {
        int _activeGunSlot = activeGunSlotInt;

        if (true) {
            if (_activeGunSlot + 1 >= gunSlots.Count)
                _activeGunSlot = 0;
            else
                _activeGunSlot++;

            if (gunSlots[_activeGunSlot].gun == null) {
                return;
            }
        }

        activeGunSlotInt = _activeGunSlot;

        if (firingStatus == FiringStatus.reloading)
            CancelReload();

        UpdateWeaponObject();
        UpdateSpread(weaponSwapped: true);
        playerManager.UpdatePlayerUIElement();
    }

    private void UpdateWeaponObject() {
        if (currentWeaponObject != null)
            Destroy(currentWeaponObject);

        GameObject gunPrefab = ActiveGunSlot.gun.gunData.gunPrefab;

        currentWeaponObject = Instantiate(gunPrefab, transform.localPosition + transform.rotation * gunPrefab.transform.localPosition, transform.localRotation * gunPrefab.transform.localRotation, transform);
    }

    //When gun is picked up, update the player inventory
    public void PickupWeapon(GameObject source, GunSlot slot) {
        for (int i = 0; i < gunSlots.Count; i++) {
            if (gunSlots[i].gun == null) {
                gunSlots[i] = slot;
                gunSlots[i].InitPool();
                Destroy(source);
                playerManager.UpdatePlayerUIElement();
                return;
            }
        }

        DropWeapon();
        gunSlots[activeGunSlotInt] = slot;
        ActiveGunSlot.InitPool();
        Destroy(source);
        playerManager.UpdatePlayerUIElement();
    }

    public void DropWeapon() {
        GameObject gun = GunPickup.CreateWeaponDrop(ActiveGunSlot, false);
        Vector3 dropLoc = transform.position + transform.forward * 1;
        GunPickup pickup = Instantiate(gun, dropLoc, Quaternion.identity).GetComponent<GunPickup>();
        pickup.StartDespawnTimer();

        ActiveGunSlot.Clear();
        playerManager.UpdatePlayerUIElement();
    }

    //Gets the shot input
    public void ShootInput(InputAction.CallbackContext context) {
        if (context.performed)
            shootButtonHeld = true;
        else if (context.canceled)
            shootButtonHeld = false;

        if (!shootButtonHeld)
            return;

        shootButtonHeld = true;

        if (ActiveGunSlot == null) return;

        Shoot();
    }

    //Swaps weapon to next active weapon slot, will roll around to the first weapon if number of held weapons is reached

    public async void ReloadInput(InputAction.CallbackContext context) => await Reload();
    public void CancelShooting() => shootButtonHeld = false;
    public GunSlot ActiveGunSlot => gunSlots[activeGunSlotInt];
    public GunSlot NextGunSlot() {
        if (gunSlots[1] == null)
            return null;

        else if (activeGunSlotInt == 0)
            return gunSlots[1];
        else
            return gunSlots[0];
    }
}

[System.Serializable]
public class GunSlot {
    public GunBase gun;

    public int currentMag;
    public int currentAmmo;

    [SerializeField] public ObjectPool<TrailRenderer> trailPool;

    public GunSlot(GunBase _gun, int _currentMag, int _currentAmmo) {
        gun = _gun;
        currentMag = _currentMag;
        currentAmmo = _currentAmmo;
    }

    public GunSlot(GunBase gunBase) : this(gunBase, gunBase.stats.maxMagSize, gunBase.stats.maxAmmo) { }

    public void Clear() {
        gun = null;
        currentMag = 0;
        currentAmmo = 0;

        FlushPool();
    }

    public void InitPool() {
        if (trailPool != null)
            FlushPool();

        trailPool = new(gun.gunTrail.CreateTrail, actionOnDestroy: gun.gunTrail.DestroyTrail);
    }
    public void FlushPool() {
        for (int i = 0; i < trailPool.CountInactive; i++) {
            trailPool.Release(trailPool.Get());
        }

        trailPool.Dispose();
    }

}

public enum FiringType {
    SelectFire,
    BurstFire,
    Automatic
}

public enum DamageType {
    generic,
    fire,
    poison
}

//Constructs data used to transfer information from weapons to enemies.
public struct HitData {
    public Vector3 hitLoc;
    public Vector3 hitNormal;

    public float damage;
    public DamageType damageType;

    public HitData(Vector3 _hitLoc, Vector3 _hitNormal, float _damage, DamageType _damageType = DamageType.generic) {
        hitLoc = _hitLoc;
        hitNormal = _hitNormal;
        damage = _damage;
        damageType = _damageType;
    }
}
