using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public enum Type
    {
        Earth,
        Water,
        Fire,
        Air,
        Physical
    }

    public class Attack
    {
        public Type type;
        public float amount;
        public Status.Effect statusEffect;
    }

    public class Resistance
    {
        public Effect effect;

        public Type type;
        public float percent;

        public enum Effect
        {
            Normal,
            Immune,
            HealDamage
        }
    }
}
