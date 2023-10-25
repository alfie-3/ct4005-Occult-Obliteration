using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;

public class SlowEffect : AreaOfEffect
{
    [SerializeField] int slowDownTime;

    public override void AffectEntity(GameObject entity, GameObject caster) {
        StartCoroutine(SlowEntity(entity));
    }
    IEnumerator SlowEntity(GameObject entity) {
        if (entity.TryGetComponent(out NavMeshAgent navAg)) {
            navAg.speed /= valueAffectAmountPerIteration;
            yield return new WaitForSeconds(slowDownTime);
            navAg.speed *= valueAffectAmountPerIteration;
        }
    }
}
