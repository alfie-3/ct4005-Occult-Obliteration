using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "New Weapon Impact Asset", menuName = "Weapons/Gun/Gun Components/Gun Impact", order = 5)]
public class GunImpactAsset : ScriptableObject {
    public GameObject enemyImpactEffect;

    ObjectPool<VisualEffect> effectPool;
    public ObjectPool<VisualEffect> EffectPool {
        get {
            effectPool ??= new ObjectPool<VisualEffect>(CreateEffect, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 10, 1000);

            return effectPool;
        }
    }

    public IEnumerator PlayEffect(Vector3 location, Vector3 normal) {
        VisualEffect effect = EffectPool.Get();

        effect.gameObject.transform.position = location;
        effect.transform.rotation = Quaternion.FromToRotation(Vector3.up, -normal) * effect.transform.rotation;

        yield return new WaitForSeconds(0.1f);

        while (effect.aliveParticleCount > 1) {
            yield return null;
        }

        effectPool.Release(effect);
    }

    void OnTakeFromPool(VisualEffect vfx) {
        vfx.gameObject.SetActive(true);
        vfx.Play();
    }

    void OnReturnedToPool(VisualEffect vfx) {
        vfx.Stop();
        vfx.gameObject.SetActive(false);
    }

    void OnDestroyPoolObject(VisualEffect vfx) {
        if (vfx)
            Destroy(vfx.gameObject);
    }

    public VisualEffect CreateEffect() {
        return Instantiate(enemyImpactEffect).GetComponent<VisualEffect>();
    }

    private void OnDisable() {
        effectPool?.Dispose();
    }
}