
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BirdEnemyController))]
public class BirdEnemyControllerEditor : Editor
{
    
    SerializedProperty stateDebug;
    SerializedProperty ENTER_RANGE;
    SerializedProperty EXIT_RANGE;
    SerializedProperty player;
    SerializedProperty animator;
    SerializedProperty speed;
    SerializedProperty damage;
    SerializedProperty debugStateText;
    SerializedProperty attackZoneGO;

    SerializedProperty attackSpeed;
    SerializedProperty moveAwaySpeed;
    SerializedProperty coolDownInterval;

    SerializedProperty pointsNum;
    SerializedProperty height;
    SerializedProperty radius;
    SerializedProperty center;
    SerializedProperty waypoints;

    private void OnEnable()
    {
        stateDebug = serializedObject.FindProperty("stateDebug");
        ENTER_RANGE = serializedObject.FindProperty("ENTER_RANGE");
        EXIT_RANGE = serializedObject.FindProperty("EXIT_RANGE");
        player = serializedObject.FindProperty("player");
        animator = serializedObject.FindProperty("animator");
        speed = serializedObject.FindProperty("speed");
        damage = serializedObject.FindProperty("damage");
        debugStateText = serializedObject.FindProperty("debugStateText");
        attackZoneGO = serializedObject.FindProperty("attackZoneGO");

        attackSpeed = serializedObject.FindProperty("attackSpeed");
        moveAwaySpeed = serializedObject.FindProperty("moveAwaySpeed");
        coolDownInterval = serializedObject.FindProperty("coolDownInterval");

        pointsNum = serializedObject.FindProperty("pointsNum");
        height = serializedObject.FindProperty("height");
        radius = serializedObject.FindProperty("radius");
        center = serializedObject.FindProperty("center");
        waypoints = serializedObject.FindProperty("waypoints");

    }

    public override void OnInspectorGUI()
    {
        BirdEnemyController birdController = (BirdEnemyController)target; 

        serializedObject.Update();
        EditorGUILayout.PropertyField(stateDebug);

        EditorGUILayout.PropertyField(player);
        EditorGUILayout.PropertyField(animator);
        EditorGUILayout.PropertyField(speed);
        EditorGUILayout.PropertyField(debugStateText);

        EditorGUILayout.PropertyField(attackSpeed);
        EditorGUILayout.PropertyField(damage);
        EditorGUILayout.PropertyField(moveAwaySpeed);
        EditorGUILayout.PropertyField(attackZoneGO);
        EditorGUILayout.PropertyField(coolDownInterval);
        EditorGUILayout.PropertyField(ENTER_RANGE);
        EditorGUILayout.PropertyField(EXIT_RANGE);

        EditorGUILayout.PropertyField(pointsNum);
        EditorGUILayout.PropertyField(height);
        EditorGUILayout.PropertyField(radius);
        EditorGUILayout.PropertyField(center);
        EditorGUILayout.PropertyField(waypoints);

        if (GUILayout.Button("Generate Waypoints"))
        {
            GenerateWaypoints(birdController);
        }
        CheckIfHasWaypoints(birdController);



        serializedObject.ApplyModifiedProperties();



        //-- BUTTON FOR UPDATING THE CENTER --//

        GUIStyle buttonTextStyle = new GUIStyle(GUI.skin.button);

        buttonTextStyle.normal.textColor = Color.yellow;
        buttonTextStyle.fontSize = 12;
        buttonTextStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Space(5);

        if (birdController.transform.position == center.vector3Value) return;
        
        if (GUILayout.Button("Update Center", buttonTextStyle))
        {
            UpdateCenter(birdController.transform);
        }
        EditorGUILayout.HelpBox("The center point is not aligned with the bird enemy. If you want the center of waypoints to be at the bird enemy's current position, please press the'Update Center' button", MessageType.Warning);

        serializedObject.ApplyModifiedProperties();
    }


    //Right now if we press play and then go back to edit mode, the waypoints GO array is null so I'll just get the children.
    private void CheckIfHasWaypoints(BirdEnemyController birdController)
    {
        if(Application.isPlaying) return;

        if (waypoints == null) return;

        Transform waypointGO = AerialWanderState.ChildIsNamed("Waypoints", birdController.transform);

        
        int waypointChildCount = waypointGO.childCount;

        if (CheckIfArrayElementsAreNull(waypoints.arraySize,birdController) && waypointGO.childCount != 0)
        {
            birdController.Waypoints = new GameObject[waypointChildCount];
         
            for (int i = 0; i < birdController.Waypoints.Length; i++)
            {
                Transform childTransform = waypointGO.GetChild(i).GetComponent<Transform>();
                birdController.Waypoints[i] = childTransform.gameObject;
            }
            
        }
    }

    private bool CheckIfArrayElementsAreNull(int childCount, BirdEnemyController birdController)
    {

        if (childCount <= 0) return false;

       
        if (birdController.Waypoints[0] == null) return true;

        return false;
        
    }
    private void GenerateWaypoints(BirdEnemyController birdController)
    {
        birdController.GenerateWaypoints();

    }
    private void UpdateCenter(Transform birdController)
    {
        center.vector3Value = birdController.position;
    }
}
