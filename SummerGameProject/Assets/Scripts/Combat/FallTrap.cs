using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTrap : MonoBehaviour
{
    //Most items are available in the editor so they can be changed by developers
    //The prefab is very bare bones so the designers can adapt it to fit anywhere
    //Potential additions... make a way for the trap to do multiple damage types.

    [SerializeField]
    BoxCollider trapTriger;

    CombatantBase damagableObject;

    [SerializeField]
    Damage.Type trapDamageType;
    [SerializeField]
    Status.Effect trapStatisEffect;
    [SerializeField]
    float trapDamageAmmout = 1;
    [SerializeField]
    float trapDamageDelayTime = 0;

    Damage.Attack trapDamage = new Damage.Attack();
    [SerializeField]
    List<Damage.Attack> damages = new List<Damage.Attack>();
    IEnumerator TrapDamageDelay()
    {
        //TO DO... play trap animation
        yield return new WaitForSeconds(trapDamageDelayTime);
        if (damagableObject != null)
        {
            damagableObject.TakeDamage(damages, damagableObject.damageResistances);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        trapDamage.amount = trapDamageAmmout;
        trapDamage.type = trapDamageType;
        trapDamage.statusEffect = null;

        damages.Add(trapDamage);        
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        damagableObject = collision.gameObject.GetComponent<CombatantBase>();

        //delay
        StartCoroutine(TrapDamageDelay());
    }
}
