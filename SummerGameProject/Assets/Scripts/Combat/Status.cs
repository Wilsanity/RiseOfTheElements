using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public enum Type
    {
        //List Diffrent Effects TO DO:
    }

    public class Effect
    {
        public Type type;
        public float chance;
    }

    public static void ApplyStatus(CombatantBase target, Type type)
    {
        switch (type)
        {
            //case: Status Effects TO DO:
            /*
            case Burn:
                //if the target is not immune to burn...
                return;
             */
            default:
                Debug.LogError($"Status Effect '{type.ToString()}' Not Recognized!");
                return;
        }
    }
}
