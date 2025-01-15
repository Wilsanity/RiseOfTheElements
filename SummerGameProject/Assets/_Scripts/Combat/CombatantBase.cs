using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CombatantBase : MonoBehaviour
{
    protected float health = 5;
    protected float maxHealth = 5;
    
    protected float overShield = 0;
    protected float maxMverShield = 0;

    public List<Damage.Resistance> damageResistances = new List<Damage.Resistance>();

    public void TakeTrueDamage(float damageAmount)
    {
        health -= damageAmount;
    }

    public void TakeDamage(List<Damage.Attack> damages, List<Damage.Resistance> resistances) 
    { 
        //For each damage type recieved
        foreach (Damage.Attack damage in damages)
        {
            //Find the damage type
            Damage.Type damageType = damage.type;
            bool damageResisted = false;

            //Search through the resistances 
            foreach (Damage.Resistance resistance in resistances)
            {
                //if found matching resistance...
                if (resistance.type == damage.type)
                {
                    switch (resistance.effect)
                    {
                        case Damage.Resistance.Effect.Normal:
                            #region normal damage resistance

                            //calculate true damageee
                            float trueDamage = damage.amount - (damage.amount * resistance.percent / 100);
                            health -= Mathf.Clamp(0, Mathf.Infinity, trueDamage);

                            //debug message for 
                            Debug.Log($"{gameObject.name} takes {trueDamage} damage!");

                            //if the damage has a status effect attached to it...
                            if (damage.statusEffect != null)
                            {
                                //if the damage has a status effect attached to it...
                                float trueStatusChance = damage.statusEffect.chance - (damage.statusEffect.chance * resistance.percent / 100);

                                //check to see if the status effect is applied
                                if (Random.Range(0, 100) > trueStatusChance)
                                {
                                    //effect the character with the status effect ("this" is the character)
                                    Status.ApplyStatus(this, damage.statusEffect.type);
                                }
                            }
                            
                            //mark damage as resisted
                            damageResisted = true;

                            #endregion
                            break;
                        case Damage.Resistance.Effect.Immune:
                            #region immunity damage resistance

                            //debug immune to damage type
                            Debug.Log($"{gameObject.name} resisted {damage.type} damage!");
                            
                            //mark damage as resisted
                            damageResisted = true;

                            #endregion
                            break;
                        case Damage.Resistance.Effect.HealDamage:
                            #region heal damage

                            //calculate true damageee
                            float trueHealing = damage.amount - (damage.amount * resistance.percent / 100);
                            health += Mathf.Clamp(0, Mathf.Infinity, trueHealing);

                            //debug message for 
                            Debug.Log($"{gameObject.name} heals for {trueHealing} health!");

                            #endregion
                            break;
                        default:
                            Debug.LogError($"Resistance Effect '{resistance.effect}' Not Recognized!");
                            break;
                    }
                } 
            }

            //if damage isn't resisted, take damages as true damage
            if (!damageResisted) health -= damage.amount;

            //if the damage has a status effect attached to it...
            if (damage.statusEffect != null)
            {
                //check to see if the status effect is applied
                if (Random.Range(0, 100) > damage.statusEffect.chance)
                {
                    //effect the character with the status effect ("this" is the character)
                    Status.ApplyStatus(this, damage.statusEffect.type);
                }
            }
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
