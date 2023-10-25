using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TestEnemy : MonoBehaviour, IHealth {
    public void Damage(HitData hitData, GameObject source) {

        if (source.TryGetComponent(out PlayerManager manager)) {
            Debug.Log(hitData.damage);
            manager.AwardStatistic("Damage Dealt", (int)hitData.damage);
        }

        HitTest();
    }

    public void Heal(float healAmount)
    {
        throw new System.NotImplementedException();
    }

    public async void HitTest() {
        GetComponent<Renderer>().material.color = Color.green;
        await Task.Delay(3000);

        if (!Application.isPlaying) {
            return;
        }

        GetComponent<Renderer>().material.color = Color.red;
    }
}
