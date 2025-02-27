using UnityEngine;
public interface IDamageable
{

    //Using ref here could be bad, Especially if our obj that damages us is pending deletion.
    public void TakeDamage(GameObject instigator, int amount = 1);


    //Don't think this ever gets used so it's fine.
    public void DealDamage(GameObject target, int amount);
}
