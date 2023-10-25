using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TrailFollow : MonoBehaviour
{
    bool following = false;

    Transform followTransform;
    Vector3 offset;

    // Start is called before the first frame update
    public void Follow(Transform _followTransform, Vector3 _offset)
    {
        following = true;
        followTransform = _followTransform;
        offset = _offset;
    }

    // Update is called once per frame
    void Update()
    {
        if (!following)
            return;

        transform.position = followTransform.position + offset;
    }

    IEnumerator DestroyTrailRoutine(float time = 0) {
        WaitForSeconds wait = new WaitForSeconds(time);
        yield return wait; 
        GetComponent<VisualEffect>().SendEvent("OnStop");
        yield return wait;
        Destroy(gameObject);
    }

    public void DestroyTrail(float time) {
        StartCoroutine(DestroyTrailRoutine(time));
    }
}
