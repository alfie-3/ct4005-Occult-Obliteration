using UnityEngine;

public interface IHealth {

    public void Damage(HitData hitData, GameObject source);

    public void Heal(float healAmount);
}
