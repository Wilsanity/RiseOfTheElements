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
    
    readonly Color c = Color.green;

    private void OnEnable()
    {
        _maxHealth = serializedObject.FindProperty("_maxHealth");
        _currentHealth = serializedObject.FindProperty("_currentHealth");
        _currentPhase = serializedObject.FindProperty("_currentPhase");
        _unitHealthPhases = serializedObject.FindProperty("_unitHealthPhases");
        _healthBar = serializedObject.FindProperty("_healthBar");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

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

        if (!Application.isPlaying)
        {
            _currentHealth.intValue = _maxHealth.intValue;
        }

        //EditorGUILayout.PropertyField(_currentHealth);
        //Current Health
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Current Health ");
        GUILayout.Label(_currentHealth.intValue.ToString(), GUILayout.MaxWidth(Screen.width / 1.8f));
        EditorGUILayout.EndHorizontal();

        //Current Phase. We only need to know this IF we have phases in the first place
        if(_unitHealthPhases.arraySize > 0)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Current Phase");
            GUILayout.Label(_currentPhase.intValue.ToString(), GUILayout.MaxWidth(Screen.width / 1.8f));
            EditorGUILayout.EndHorizontal();
        }
        




        EditorGUILayout.PropertyField(_unitHealthPhases);

        GUILayout.Label("Unit Health Bar", titleStyle);

        //Draw the Health bar (Behind)
        Rect healthPhasesArrayRect = new Rect(GUILayoutUtility.GetLastRect().x, GUILayoutUtility.GetLastRect().y, GUILayoutUtility.GetLastRect().width, GUILayoutUtility.GetLastRect().height);
        EditorGUI.DrawRect(new Rect(20, healthPhasesArrayRect.y + healthPhasesArrayRect.height + 40, healthPhasesArrayRect.width - 20, 30), Color.black);
       
        //Draw the Current health health bar
        EditorGUI.DrawRect(new Rect(20, 
            healthPhasesArrayRect.y + healthPhasesArrayRect.height + 40, 
            (healthPhasesArrayRect.width - 20) / ((float)_maxHealth.intValue / (float)_currentHealth.intValue), 
            30), c);
        
        float widthOfHealthBar = healthPhasesArrayRect.width - 20;
        GUILayout.Space(150);


        //Draw the Min Vale and Max Value and the white rectangles at the end points
        Rect healthBarMinValueRect = new Rect(20, healthPhasesArrayRect.y + (healthPhasesArrayRect.height + 70), 10, 50);
        Rect healthBarMaxValueRect = new Rect(healthPhasesArrayRect.width - 5, healthPhasesArrayRect.y + (healthPhasesArrayRect.height + 70), 50, 50);
        EditorGUI.LabelField(healthBarMinValueRect, "0");
        EditorGUI.LabelField(healthBarMaxValueRect, _maxHealth.intValue.ToString());
        EditorGUI.DrawRect(new Rect(healthBarMinValueRect.x, healthBarMinValueRect.y + 5, 2, -40), Color.white);
        EditorGUI.DrawRect(new Rect(healthPhasesArrayRect.width, healthBarMaxValueRect.y + 5, 2, -40), Color.white);
        
        
        
        


        // This is in charge of drawing the boss health increment bars based on the the healthPhases array size and data
        //I have to put this code after we apply the modified properties or else we get an error since the array of the property doesn't update on time
        for (int i = 0; i < _unitHealthPhases.arraySize; i++)
        {
            
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


            //if (boss.B_BossPhases[i].phaseEvent != null)
            //{
            //    textStyle.normal.textColor = new Color(0.2f, 0.69f, 0.92f, 1);

            //    //Drawing the Event text for the phase
            //    EditorGUI.LabelField(new Rect(incrementBar.x - 15, incrementBar.y + 66, 10, 50), "Event", textStyle);
            //}


            
        }
        serializedObject.ApplyModifiedProperties();

    }

}
