using UnityEngine;
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

    //Properties
    public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    public int MaxHealth { get => _maxHealth;}
    public UnitHealthPhases[] UnitHealthPhases { get => _unitHealthPhases; set => _unitHealthPhases = value; }
    public int CurrentPhase { get => _currentPhase;}
    public Image HealthBar { get => _healthBar;}



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

    //I'm going to make this changeable for all enemies and player. Maybe make a scriptable Object with certain death logic
    private void PerformDeathLogic()
    {
        if(gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

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
        float healthPercent = (_currentHealth / _maxHealth * 100);

        //loop through the phases

        //if lower than one, set that phase

        //For loop to see if it is > the current increment or not
        for (int i = _unitHealthPhases.Length - 1; i >= 0; i--)
        {

            //eg. if the current health is 65%, then we want to check if <25, then <50, then <75, then <100

            if (healthPercent > _unitHealthPhases[i].phaseHealthPercent)
                continue;


            //Play the special Health Increment event if this phase has one and it hasn't already played
            //if (_unitHealthPhases[i].phaseEvent != null)
            //{
            //    if (!bossProfile.B_BossPhases[i].phaseEvent.EventPlayed)
            //        bossProfile.B_BossPhases[i].phaseEvent.OnHealthChange();
            //}

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

}
