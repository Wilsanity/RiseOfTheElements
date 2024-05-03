using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This is a display script to show off what a phase event can look like. You will need to make separate scripts for the player, enemies, bosses, etc. if 
/// you want different logic for each one. You can inherit from this script, just make sure you make a new Asset Menu.
/// </summary>

[CreateAssetMenu(fileName = "Unit Phase Event", menuName = "Scriptable Objects/Health Phase Event")]
public class SO_BaseUnitPhaseEvent : ScriptableObject
{
    //THis variable will be set in the 'UnitHealth.cs' script so we can access scirpts in this gameObject
    private GameObject unitAssigned;
    private bool[] completedEvents = new bool[5];

    
    public void Initialize(GameObject unit)
    {
        completedEvents = new bool[5];
        unitAssigned = unit;
    }


    public virtual void HealthPhase1Event()
    {
        int i = 0;
        if (IsCompleted(i)) return;

        Debug.Log("Performed phase 1 event");
    }
    public virtual void HealthPhase2Event()
    {
        int i = 1;
        if (IsCompleted(i)) return;

        Debug.Log("Performed phase 2 event");
    }
    public virtual void HealthPhase3Event()
    {
        int i = 2;
        if (IsCompleted(i)) return;

        Debug.Log("Performed phase 3 event");
    }
    public virtual void HealthPhase4Event()
    {
        int i = 3;
        if (IsCompleted(i)) return;

        Debug.Log("Performed phase 4 event");
    }
    public virtual void DefeatedLogic()
    {
        int i = 4;
        if(IsCompleted(i)) return;

        Debug.Log("Performed defeat");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Destroy(unitAssigned);
    }

    private bool IsCompleted(int index)
    {
        //If we already did the logic for an event, we shouldn't do it again. This makes sure the code doesn't
        //get executed more than once. 
        if (completedEvents[index]) return true; 

        completedEvents[index] = true;
        return false;
    }

}
