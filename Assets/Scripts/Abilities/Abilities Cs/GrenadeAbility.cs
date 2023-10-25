//Ability that throws any grenade prefab

using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[CreateAssetMenu(fileName = "New Grenade Ability", menuName = "Abilities/Grenade Ability", order = 1)]
public class GrenadeAbility : Ability {
    [SerializeField] GameObject grenadePrefab;
    [Space]
    [MinMaxSlider(1, 100, FlexibleFields = true), SerializeField] Vector2 throwStrengthRange = new(1, 12);
    [Range(0f, 180f), SerializeField] float throwAngle = 45;
    float currentThrowStrength = 1;

    float timeBetweenPoints = 0.2f;
    int linePoints = 30;
    LineRenderer lineRenderer;
    LayerMask lineMask;

    [Space]
    [SerializeField] LineRendererSettings lineRendererSettings;
    [SerializeField] GameObject landMarkerDecalPrefab;

    DecalProjector decal;

    //Shows grenade trajectory and explosion radius when the grenade button is held
    public override void ButtonHeld(GameObject caster) {
        base.ButtonHeld(caster);
        if (lineRenderer == null) {
            currentThrowStrength = throwStrengthRange.x;
            CreateTrajectoryEffect(caster);
        }

        DrawTrajectory(caster);

        if (currentThrowStrength < throwStrengthRange.y)
            currentThrowStrength += 10 * Time.deltaTime;
        else
            currentThrowStrength = throwStrengthRange.y;
    }

    //Grenade thrown on ability button release
    public override bool Activate(GameObject caster) {
        base.Activate(caster);
        ThrowGrenade(caster);

        Destroy(lineRenderer.gameObject);
        Destroy(decal.gameObject);
        lineRenderer = null;
        decal = null;

        return true;
    }

    //Draws trajectory of grenade
    private void DrawTrajectory(GameObject caster) {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = Mathf.CeilToInt(linePoints / timeBetweenPoints) + 1;

        Vector3 startPos = caster.transform.position;
        Vector3 startVelocity = currentThrowStrength * (GrenadeTrajectory(caster.transform) / grenadePrefab.GetComponent<Rigidbody>().mass);

        int i = 0;
        lineRenderer.SetPosition(i, startPos);

        for (float time = 0; time < linePoints; time += timeBetweenPoints) {
            i++;
            Vector3 point = startPos + time * startVelocity;
            point.y = startPos.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            lineRenderer.SetPosition(i, point);

            Vector3 lastPos = lineRenderer.GetPosition(i - 1);

            if (Physics.Raycast(lastPos, (point - lastPos).normalized, out RaycastHit hit, (point - lastPos).magnitude, lineMask)) {
                lineRenderer.SetPosition(i, hit.point);
                decal.transform.position = hit.point;
                lineRenderer.positionCount = i + 1;
                return;
            }
        }
    }

    //Adds initial force to the grenade
    private void ThrowGrenade(GameObject caster) {
        GameObject grenade = Instantiate(grenadePrefab, caster.transform.position, Quaternion.identity);
        if (grenade.GetComponent<Grenade>() != null) {
            grenade.GetComponent<Grenade>().caster = caster;
        }
        else if (grenade.GetComponent<HolyGrenadeObject>() != null) {
            grenade.GetComponent<HolyGrenadeObject>().caster = caster;
        }
        grenade.GetComponent<Rigidbody>().AddForce(GrenadeTrajectory(caster.transform) * currentThrowStrength, ForceMode.Impulse);
    }

    //Creates visible trajectory for player
    private void CreateTrajectoryEffect(GameObject caster) {
        GameObject instance = new("GrenadeThrowPoint");
        instance.transform.position = caster.transform.position;
        lineRenderer = instance.AddComponent<LineRenderer>();
        SetLineRendererSettings();

        int grenadeLayer = grenadePrefab.gameObject.layer;

        for (int i = 0; i < 32; i++) {
            if (!Physics.GetIgnoreLayerCollision(grenadeLayer, i)) {
                lineMask |= 1 << i;
            }
        }

        decal = Instantiate(landMarkerDecalPrefab).GetComponent<DecalProjector>();
        if (grenadePrefab.GetComponent<BaseGrenade>() != null) {
            BaseGrenade grenadeBase = grenadePrefab.GetComponent<BaseGrenade>();

            decal.size = new(grenadeBase.GetRadius * 2, grenadeBase.GetRadius * 2, 2);
            decal.transform.localScale = new(grenadeBase.GetRadius * 2, grenadeBase.GetRadius * 2, grenadeBase.GetRadius * 2);
        }
    }

    //Trajectory maths for grenade path
    public Vector3 GrenadeTrajectory(Transform casterTransform) {
        var a = throwAngle * Mathf.Deg2Rad;
        return casterTransform.forward * Mathf.Cos(a) + casterTransform.up * Mathf.Sin(a);
    }

    private void SetLineRendererSettings() {
        lineRenderer.material = lineRendererSettings.material;
        lineRenderer.colorGradient = lineRendererSettings.color;
        lineRenderer.startWidth = lineRendererSettings.width;
        lineRenderer.endWidth = lineRendererSettings.width;
    }

    [Serializable]
    public class LineRendererSettings {
        public Material material;
        public Gradient color;
        public float width;
    }
}

