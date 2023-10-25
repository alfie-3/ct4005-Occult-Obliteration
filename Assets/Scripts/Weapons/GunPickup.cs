//Guns to pick up when interacted with

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GunPickup : Interactable {
    public static readonly string GunPickupPrefabDirectory = "Prefabs/GunPickup";
    private static GunPickup gunPickupPrefab;
    private static WeaponRarityObject weaponRarityTable;

    [Header("GunPickup")]
    public GunSlot slot;
    [SerializeField] private GameObject statCardPrefab;
    [Space]
    public bool purchasable;
    [Space]
    [SerializeField] AudioClip[] audioClips;
    int cost;

    WeaponStatCard currentCard;
    GameObject currentModel;

    [RuntimeInitializeOnLoadMethod]
    private static void LoadWeaponDrop() {
        gunPickupPrefab = (Resources.Load(GunPickupPrefabDirectory) as GameObject).GetComponent<GunPickup>();
        weaponRarityTable = Resources.Load("Scriptables/Weapon Rarity Table") as WeaponRarityObject;
    }

    //Start the timer and sets values for the item
    public void Start() {
        cost = slot.gun.gunData.price;

        if (slot == null)
            return;

        SetRarityAttributes();
        InstantiateModel();
    }

    public GunPickup(GunBase gunBase, int currentMag, int currentAmmo, bool _purchasable = false) {
        slot = new(gunBase, currentMag, currentAmmo);
        purchasable = _purchasable;
    }

    public GunPickup(GunBase gunBase, bool _purchasable = false) : this(gunBase, gunBase.stats.maxMagSize, gunBase.stats.maxAmmo, _purchasable = false) {
        purchasable = _purchasable;
    }

    //Creates a gun that can be picked up
    public static GameObject CreateWeaponDrop(GunSlot gunSlot, bool forPuchase) {
        gunPickupPrefab.slot = gunSlot;
        gunPickupPrefab.purchasable = forPuchase;

        return gunPickupPrefab.gameObject;
    }

    //Set the colour based on the rarity of the weapon
    public void SetRarityAttributes() {
        GetComponent<Light>().color = weaponRarityTable.weaponRarities[(int)slot.gun.gunData.weaponRarity].lightColour;
        GetComponentInChildren<MeshRenderer>().material = weaponRarityTable.weaponRarities[(int)slot.gun.gunData.weaponRarity].material;
    }

    //Creates gameobject pickup for player
    public void InstantiateModel() {
        if (!slot.gun)
            return;

        if (currentModel)
            DestroyImmediate(currentModel);

        currentModel = Instantiate(slot.gun.gunData.gunPrefab.GetComponentInChildren<MeshRenderer>().gameObject, transform);
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.rotation = Quaternion.Euler(0, 90, 42);
    }

    //Function for when object is interacted with
    public override void Interact(GameObject source) {
        if (purchasable) {
            if (source.GetComponent<PlayerManager>().souls - cost < 0)
                return;
            else
                source.GetComponent<PlayerManager>().ModifySouls(cost, PlayerManager.SoulModification.remove);

            source.GetComponent<AudioSource>().PlayOneShot(audioClips[1], 0.5f);
        }

        source.TryGetComponent(out WeaponController controller);
        if (controller != null)
            controller.PickupWeapon(gameObject, slot);

        source.GetComponent<AudioSource>().PlayOneShot(audioClips[0]);

        UnView();
    }

    //Destroy visible info card when not looking at gameobject
    public override void UnView() {
        base.UnView();

        Destroy(currentCard.gameObject);
        currentCard = null;
    }

    //Creates info card when within interact distance
    public override void View(PlayerInput viewerInput) {
        if (viewed || currentCard != null)
            return;


        var managerTrsfm = UIManager.Current.transform;
        RectTransform managerRect = UIManager.Current.GetComponent<RectTransform>(); ;

        currentCard = Instantiate(statCardPrefab, managerTrsfm.position, managerTrsfm.rotation, UIManager.Current.transform).GetComponent<WeaponStatCard>();

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        Vector2 rectPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(managerRect, screenPoint, Camera.main, out rectPoint);
        rectPoint.y += 150;
        currentCard.GetComponent<RectTransform>().anchoredPosition = rectPoint;

        currentCard.Init(slot.gun, viewerInput, purchasable);

        base.View(viewerInput);
    }

    //If the item is not picked up it will self destruct
    public void StartDespawnTimer() {
        Invoke(nameof(Despawn), 20);
    }

    public void Despawn() {
        Destroy(gameObject);
    }

    //Destroys card when the object is destroyed too
    private void OnDestroy() {
        if (currentCard != null)
            Destroy(currentCard.gameObject);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GunPickup))]
[CanEditMultipleObjects]
public class LookAtPointEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        GunPickup gunPickup = (GunPickup)target;

        //Adds a button to fill ammo to max
        if (GUILayout.Button("Populate Pickuo")) {
            gunPickup.slot.currentMag = gunPickup.slot.gun.stats.maxMagSize;
            gunPickup.slot.currentAmmo = gunPickup.slot.gun.stats.maxAmmo;

            gunPickup.Start();

            PrefabUtility.RecordPrefabInstancePropertyModifications(gunPickup);
        }
    }
}
#endif

