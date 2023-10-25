using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Weapon Trail Asset", menuName = "Weapons/Gun/Gun Components/Gun Trail", order = 5)]
public class GunTrailAsset : ScriptableObject {

    public Material material;
    public AnimationCurve widthCurve;
    public float duration = 0.5f;
    public float minVertexDistance = 0.1f;
    public Gradient color;
    [Space]
    public float simulationSpeed = 100;
    [Space]
    public GunImpactAsset enemyHitEffect;

    public IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit, ObjectPool<TrailRenderer> trailPool) {

        TrailRenderer instance = trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPoint;
        yield return null;

        instance.emitting = true;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0) {
            instance.transform.position = Vector3.Lerp(startPoint, endPoint, Mathf.Clamp01(1 - (remainingDistance / distance)));

            remainingDistance -= simulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = endPoint;

        yield return new WaitForSeconds(duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        trailPool.Release(instance);
    }

    public TrailRenderer CreateTrail() {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();

        trail.colorGradient = color;
        trail.material = material;
        trail.widthCurve = widthCurve;
        trail.time = duration;
        trail.minVertexDistance = minVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    public void DestroyTrail(TrailRenderer system) {
        if (system == null) { return; }
        DestroyImmediate(system.gameObject);
    }
}
