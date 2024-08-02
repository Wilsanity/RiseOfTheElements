using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UnitHealth : MonoBehaviour
{
    //Stores the max health, current health, and phases if the enemy has.

    //Fields
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _currentPhase = 0;

    [SerializeField] private UnitHealthPhases[] _unitHealthPhases = new UnitHealthPhases[0];

    [SerializeField] private Image _healthBar;

    [SerializeField] private UnityEvent _deathEvent;

    //Properties
    public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    public int MaxHealth { get => _maxHealth;}
    public UnitHealthPhases[] UnitHealthPhases { get => _unitHealthPhases; set => _unitHealthPhases = value; }
    public int CurrentPhase { get => _currentPhase;}
    public Image HealthBar { get => _healthBar;}

    private void Start()
    {
        //Initialize Events for the boss phases
        for(int i = 0; i < _unitHealthPhases.Length; i++)
        {
            //Check to see if we have an event. If we don't it'll give us an error index out of range so make sure to continue if have none.
            bool isEventRegistered = _unitHealthPhases[i].unitPhaseEvent.GetPersistentEventCount() > 0 ? true : false;

            if (!isEventRegistered) continue;

            Object obj = _unitHealthPhases[i].unitPhaseEvent.GetPersistentTarget(0);

            //Check to see if the object we put in the inspector event is an SO_BaseUnitPhaseEvent
            SO_BaseUnitPhaseEvent so = obj as SO_BaseUnitPhaseEvent;

            if (so != null)
            {
                //Initialize the event
                so.Initialize(gameObject);
            }
        }

        //Initialize with the death event too
        //Check to see if we have an event. If we don't it'll give us an error index out of range so make sure to continue if have none.
        bool getCount = _deathEvent.GetPersistentEventCount() > 0 ? true : false;

        if (!getCount) return;
        Object obj2 = _deathEvent.GetPersistentTarget(0);

        //Check to see if the object we put in the inspector event is an SO_BaseUnitPhaseEvent
        SO_BaseUnitPhaseEvent so2 = obj2 as SO_BaseUnitPhaseEvent;

        if (so2 != null)
        {
            //Initialize the event
            so2.Initialize(gameObject);
        }
    }

    //Deal Damage to the unit this script is attached to
    public void DamageUnit(int damageAmount)
    {
        if (_currentHealth <= 0) return;

        //reduce health and make sure we can't go into the negatives
        _currentHealth = Mathf.Clamp(_currentHealth - damageAmount, 0, _maxHealth);
        Debug.Log($"{gameObject.name} took {damageAmount} damage");


        if (_currentHealth <= 0)
        {
            PerformDeathLogic();
            
        }

        UpdateHealthBar();


        //Now update the current unit's phase based on the new health
        if (_unitHealthPhases.Length != 0) _currentPhase = SetCurrentHealthPhase();
    }

    //This code can now be universally shared with any unit we put the script on. We can code custom defeated logic in a separate
    //scriptable object
    private void PerformDeathLogic()
    {
        if(_deathEvent.GetPersistentEventCount() != 0)
        {
            _deathEvent.Invoke();
        }
        else
        {
            //Default to just destroying the unit if we haven't coded any extra defeated logic.
            DestroyObject();
        }
        
    }

    //I make this public in case we want to access it through animation events
    public void DestroyObject()
    {
        //Destroy Unit
        Destroy(gameObject);
    }


    //Heal the unit this script is attached to
    public void HealUnit(int healAmount)
    {
        if (_currentHealth >= _maxHealth) return;

        //heal the unit and make sure their health can't go over their maxHealth
        _currentHealth = Mathf.Clamp(_currentHealth + healAmount, 0, _maxHealth);
        Debug.Log($"{gameObject.name} healed {healAmount} HP");

        UpdateHealthBar();

        //Now update the current unit's phase based on the new health
        if (_unitHealthPhases.Length != 0) _currentPhase = SetCurrentHealthPhase();
    }


    //Mainly for bosses, get the current phase the unit is at if it has any
    private int SetCurrentHealthPhase()
    {
        
        //Get percent health from max health
        float healthPercent = ((float)_currentHealth / (float)_maxHealth * 100.00f);

        //loop through the phases

        //if lower than one, set that phase

        //For loop to see if it is > the current increment or not
        for (int i = _unitHealthPhases.Length - 1; i >= 0; i--)
        {

            //eg. if the current health is 65%, then we want to check if <25, then <50, then <75, then <100
            
            if (healthPercent > _unitHealthPhases[i].phaseHealthPercent)
                continue;

            //Debug.Log($"Paseed Phase {(i+1)}");
            //Play the special Health Increment event if this phase has one and it hasn't already played
            if (_unitHealthPhases[i].unitPhaseEvent != null)
            {
                //Debug.Log("PERFORMING EVENT");
                _unitHealthPhases[i].unitPhaseEvent.Invoke();
            }

            return i + 1;
        }

        return 0;
    }

    private void UpdateHealthBar()
    {
        if (_healthBar != null)
        {
            _healthBar.fillAmount =  1.00f / ((float)_maxHealth / (float)_currentHealth);
        }
    }


}

[System.Serializable]
public class UnitHealthPhases
{
    [HideInInspector] public string phaseName = "Phase ";

    [Range(0, 100)]
    public float phaseHealthPercent = 55;

    public UnityEvent unitPhaseEvent = null;

}
