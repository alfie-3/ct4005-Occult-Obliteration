using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Weapon Rarities", menuName = "Weapons/Gun/Weapon Rarities", order = 0)]
public class WeaponRarityObject : ScriptableObject {
   [SerializeField] public WeaponRarityData[] weaponRarities;
}

public enum WeaponRarity {
    Common = 0,
    Rare = 1,
    Legendary = 2
}

[System.Serializable]
public class WeaponRarityData {
   public Color lightColour = Color.green;
   public Material material;
   public VisualEffect effect;
}
