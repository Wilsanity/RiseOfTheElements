using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitHealth))]
public class UnitHealthEditor : Editor
{
    SerializedProperty _maxHealth;
    SerializedProperty _currentHealth;
    SerializedProperty _currentPhase;
    SerializedProperty _unitHealthPhases;
    SerializedProperty _healthBar;
    SerializedProperty phaseName;
    SerializedProperty phaseHealthPercent;
    SerializedProperty _deathEvent;

    readonly Color c = Color.green;
    private float noPhaseGap = 0; //gap between the "Unit Health Bar" title and the actual health bar should be lower if no phases and more if phases
    private void OnEnable()
    {
        _maxHealth = serializedObject.FindProperty("_maxHealth");
        _currentHealth = serializedObject.FindProperty("_currentHealth");
        _currentPhase = serializedObject.FindProperty("_currentPhase");
        _unitHealthPhases = serializedObject.FindProperty("_unitHealthPhases");
        _healthBar = serializedObject.FindProperty("_healthBar");
        _deathEvent = serializedObject.FindProperty("_deathEvent");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        UnitHealth uh = (UnitHealth)target;

        //Text Styles
        GUIStyle textStyle = new GUIStyle();
        textStyle.fontSize = 16;
        textStyle.normal.textColor = Color.white;

        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = 16;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.white;
        titleStyle.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.PropertyField(_maxHealth);
        EditorGUILayout.PropertyField(_healthBar);
        

        // -- CURRENT HEALTH -- //

        if (!Application.isPlaying)
        {
            _currentHealth.intValue = _maxHealth.intValue;
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Current Health ");
        GUILayout.Label(_currentHealth.intValue.ToString(), GUILayout.MaxWidth(Screen.width / 1.8f));
        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.PropertyField(_currentHealth);

        // -- CURRENT PHASE -- //

        //Current Phase. We only need to know this IF we have phases in the first place
        if (_unitHealthPhases.arraySize > 0)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Current Phase");
            GUILayout.Label(_currentPhase.intValue.ToString(), GUILayout.MaxWidth(Screen.width / 1.8f));
            EditorGUILayout.EndHorizontal();
            noPhaseGap = 0;
        }
        else
        {
            noPhaseGap = 20;
        }
        


        // -- HEALTH PHASES -- //

        EditorGUILayout.PropertyField(_unitHealthPhases);


        // -- WARNING FOR HEALTH PHASES -- //
        for(int i = _unitHealthPhases.arraySize -1; i >= 0 ; i--)
        {
            if (i == 0) break;
            //if one phase is past another, than that's no good.
            SerializedProperty currentPhaseHealthPercent = _unitHealthPhases.GetArrayElementAtIndex(i).FindPropertyRelative("phaseHealthPercent");
            SerializedProperty nextPhaseHealthPercent = _unitHealthPhases.GetArrayElementAtIndex(i - 1).FindPropertyRelative("phaseHealthPercent");

            if(currentPhaseHealthPercent.floatValue > nextPhaseHealthPercent.floatValue)
            {
                EditorGUILayout.HelpBox($"Phase {i+1} must be a lower value than Phase {i}", MessageType.Warning);
            }
        }



        // -- HEALTH BAR -- //
        GUILayout.Label($"{uh.gameObject.name} Health Bar", titleStyle);

        //Draw the Health bar (Behind)
        //The reason why I split them into their components is cause if we want to adjust the placement of the health bar drawing, all we have to do is  + or - the y value only
        Rect healthPhasesArrayRect = new Rect(GUILayoutUtility.GetLastRect().x, GUILayoutUtility.GetLastRect().y - noPhaseGap, GUILayoutUtility.GetLastRect().width, GUILayoutUtility.GetLastRect().height);
        EditorGUI.DrawRect(new Rect(20, healthPhasesArrayRect.y + healthPhasesArrayRect.height + 40, healthPhasesArrayRect.width - 20, 30), Color.black);

        float currentHealthPositionOnHealthBar = (healthPhasesArrayRect.width - 20) / ((float)_maxHealth.intValue / (float)_currentHealth.intValue);
        //Draw the Current health health bar
        EditorGUI.DrawRect(new Rect(20, 
            healthPhasesArrayRect.y + healthPhasesArrayRect.height + 40, currentHealthPositionOnHealthBar, 30), c);
        
        float widthOfHealthBar = healthPhasesArrayRect.width - 20;



        


        GUILayout.Space(150);



        // -- HEALTH BAR MIN AND MAX VALUES -- //

        //Draw the Min Vale and Max Value and the white rectangles at the end points
        Rect healthBarMinValueRect = new Rect(10, healthPhasesArrayRect.y + (healthPhasesArrayRect.height + 70), 50, 50);
        Rect healthBarMaxValueRect = new Rect(healthPhasesArrayRect.width - 20, healthPhasesArrayRect.y + (healthPhasesArrayRect.height + 70), 50, 50);
        EditorGUI.LabelField(healthBarMinValueRect, "0 HP");
        EditorGUI.LabelField(healthBarMaxValueRect,$" {_maxHealth.intValue} HP");
        EditorGUI.DrawRect(new Rect(healthBarMinValueRect.x + 10, healthBarMinValueRect.y + 5, 2, -40), Color.white);
        EditorGUI.DrawRect(new Rect(healthPhasesArrayRect.width, healthBarMaxValueRect.y + 5, 2, -40), Color.white);
        
        //display the current HP under the green health bar if we're not at the ends
        if(_currentHealth.intValue != _maxHealth.intValue && _currentHealth.intValue != 0)
        {
            EditorGUI.LabelField(new Rect(currentHealthPositionOnHealthBar + 5, healthBarMinValueRect.y - 10, 50, 50), $"{_currentHealth.intValue} HP");
        }


        EditorGUILayout.PropertyField(_deathEvent);

        // -- HEALTH PHASES BARS -- //

        // This is in charge of drawing the boss health increment bars based on the the healthPhases array size and data
        //I have to put this code after we apply the modified properties or else we get an error since the array of the property doesn't update on time
        for (int i = 0; i < _unitHealthPhases.arraySize; i++)
        {
            textStyle.normal.textColor = Color.white;
            phaseHealthPercent = _unitHealthPhases.GetArrayElementAtIndex(i).FindPropertyRelative("phaseHealthPercent");
            

            //Change the name of the phase in the inspector array
            phaseName = _unitHealthPhases.GetArrayElementAtIndex(i).FindPropertyRelative("phaseName");

            phaseName.stringValue = $"Phase {i + 1}";
            //Loop through each health percent and draw a rect

            float f = phaseHealthPercent.floatValue;

            Rect incrementBar = new Rect((f * (widthOfHealthBar / 100)) + 20, healthPhasesArrayRect.y + healthPhasesArrayRect.height + 30, 8, 50);

            //Drawing the Ticks for the health percentages
            EditorGUI.DrawRect(incrementBar, new Color(c.r - 0.3f, c.g - 0.3f, c.b - 0.3f));


            //Drawing the Number text for the health percentages
            EditorGUI.LabelField(new Rect(incrementBar.x - 10, incrementBar.y + 52, 10, 50), $"{f.ToString("00")}%", textStyle);

            //Drawing the phase number above the rectangle tick
            EditorGUI.LabelField(new Rect(incrementBar.x - 25, incrementBar.y - 20, 10, 50), $"Phase {i + 1}", textStyle);


            // -- EVENT CODE -- //
            
            int eventCount = uh.UnitHealthPhases[i].unitPhaseEvent.GetPersistentEventCount();
            if (eventCount > 0)
            {
                textStyle.normal.textColor = new Color(0.2f, 0.69f, 0.92f, 1);

                //Drawing the Event text for the phase
                EditorGUI.LabelField(new Rect(incrementBar.x - 15, incrementBar.y + 66, 10, 50), "Event", textStyle);
            }

        }


        serializedObject.ApplyModifiedProperties();

    }

}
