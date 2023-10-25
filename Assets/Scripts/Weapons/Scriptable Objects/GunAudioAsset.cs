using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Audio Asset", menuName = "Weapons/Gun/Gun Components/Gun Audio", order = 4)]
public class GunAudioAsset : ScriptableObject {
    public AudioClip fireClip;
    public AudioClip reloadClip;
}