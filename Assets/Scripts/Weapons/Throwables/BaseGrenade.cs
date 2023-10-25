//Base class for all grenades

using UnityEngine;

public abstract class BaseGrenade : MonoBehaviour {
    [Header("Grenade Values")]
    public int damage;
    public float fuseDuration;
    public float explosionRadius;
    public GameObject explosion;
    public LayerMask layerMask;
    public AudioClip explosionClip;
    public GameObject grenadeModel;

    [HideInInspector] public GameObject caster;

    // Start is called before the first frame update
    
    //Detonation results to be overwritten by different inheritors
    public virtual void Detonate() {}

    public float GetRadius => explosionRadius;
}
