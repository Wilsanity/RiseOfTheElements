using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Collections;

public enum SceneTransitionType
{
    Default
}
public class SceneTransitionController : MonoBehaviour
{
    public static SceneTransitionController Instance;
    //Components
    private Animation animationComp;

    //Variables
    [SerializeField] private List<TransitionTypes> arrTransitionTypes; // Array to store Animation Game Objects

    [SerializeField] private AnimationClip animationClipToPlay;

    private bool isAnimationDone; // Flag to check if the animation is done

    private int transitionTypeEnumLength = 0;

    private GameObject prevGO; // the previous game Object animation

    //Properties
    public bool IsAnimationDone1 { get => isAnimationDone; set => isAnimationDone = value; }
    public AnimationClip AnimationClipToPlay { get => animationClipToPlay; }
    public Animation AnimationComp { get => animationComp; set => animationComp = value; }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        animationComp = GetComponent<Animation>();

    }

   
    void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from the sceneLoaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        PlayEnterSceneAnimation();
    }


    // Function to play the loading animation
    public void PlayExitSceneAnimation()
    {
        GameObject go = GetSceneTransitionGameObject();

        if (go == null) return;

        go.SetActive(true);

        prevGO = go;

        ISceneTransition ist = go.GetComponent<ISceneTransition>();

        if (!HasInterfaceComponent(go, ist)) return;

        PlayClip(ist.ExitSceneAnimation);

    }

    // Function to play the loaded animation
    public void PlayEnterSceneAnimation()
    {
        animationComp.Stop();

        GameObject go = GetSceneTransitionGameObject();

        if (go == null) return;

        if(prevGO == null)
        {
            go.SetActive(true);
        }

        ISceneTransition ist = go.GetComponent<ISceneTransition>();

        if (!HasInterfaceComponent(go, ist)) return;

        PlayClip(ist.EnterSceneAnimation);
        
    }

    private void PlayClip(AnimationClip transitionAnimation)
    {
        animationClipToPlay = transitionAnimation;
        animationComp.clip = animationClipToPlay;
        animationComp.Play();
    }

    //Function to Check if the object put into the arrTransitionTypes list has the ISceneTransition script.
    //This script is needed to store Animation Clips and perform logic for they scene transition's animation.
    private bool HasInterfaceComponent(GameObject go, ISceneTransition ist)
    {
        if (ist == null)
        {
            Debug.LogWarning($"{go} does not have a script that implements ISceneTransition attached to it. \n" +
                             $"Please add a script that implements the ISceneTransition interface");
            return false;
        }

        return true;
    }

    //Called from the SceneTransitionControllerEditor.cs class
    //Updates the arrTransitionTypes List to the amount of enum values in the TransitionType enum in the MsgOperator.cs class
    //sets the name of each element in the list to the name of the enum value at the index
    public void UpdateTransitionTypeArray()
    {

        transitionTypeEnumLength = Enum.GetValues(typeof(SceneTransitionType)).Length;

        //Create a List of the transition types
        List<TransitionTypes> temp = new List<TransitionTypes>();
        for (int j = 0; j < transitionTypeEnumLength; j++)
        {
            temp.Add(new TransitionTypes());
        }

        int index = 0;

        foreach (string name in Enum.GetNames(typeof(SceneTransitionType)))
        {

            temp[index]._transitionTypeName = name;

            for (int j = 0; j < arrTransitionTypes.Count; j++)
            {
                if (temp[index]._transitionTypeName.Equals(arrTransitionTypes[j]._transitionTypeName))
                {
                    temp[index]._transitionGameObject = arrTransitionTypes[j]._transitionGameObject;
                }
            }
            index++;
        }


        arrTransitionTypes = temp;
    }

    public GameObject GetSceneTransitionGameObject()
    {
        SceneTransitionType tt = GetTransition();

        int index = 0;

        //Find the GameObject associated with the TransitionType
        foreach (string name in Enum.GetNames(typeof(TransitionType)))
        {
            
            if (tt.ToString().Equals(name))
            {
                return arrTransitionTypes[index]._transitionGameObject;
            }
            
            index++;
        }

        return arrTransitionTypes[0]._transitionGameObject;

    }

    // Method to get the transition we're going to play
    public SceneTransitionType GetTransition()
    {
        return SceneTransitionType.Default;
    }

    // Function to check if the animation is done
    public bool IsAnimationDone()
    {
        return isAnimationDone;
    }

    //The order of Operations:
    // 1. The OnSceneLoaded Event activates calling the PlayEnterSceneAnimation() Method
    // 2. The PlayEnterSceneAnimation() hides the previous transition object
    // 3. The Animation Event calls on the FinishedSceneAnimation() method which sets the current transition object to visible
    public void FinishedSceneAnimation()
    {
        if (GetSceneTransitionGameObject() == null) return;

        GetSceneTransitionGameObject().SetActive(true);

        if (prevGO != null && prevGO != GetSceneTransitionGameObject())
        {
            prevGO.SetActive(false);
        }
    }

    public void HideTransitionGO()
    {
        if (GetSceneTransitionGameObject() == null) return;
        GetSceneTransitionGameObject().SetActive(false);
    }

    // Method to get all child game objects
    private GameObject[] GetChildGameObjects()
    {
        // Get all components in children, including inactive ones
        Transform[] childTransforms = GetComponentsInChildren<Transform>(true);

        // Create a new array of game objects to store the children
        GameObject[] childObjects = new GameObject[childTransforms.Length - 1];

        int index = 0;
        foreach (Transform childTransform in childTransforms)
        {
            // Skip the parent game object itself
            if (childTransform != transform)
            {
                childObjects[index] = childTransform.gameObject;
                index++;
            }
        }

        return childObjects;
    }

    public void LoadSpecificSceneBuildIndex(int buildIndex)
    {
        StartCoroutine(WaitForAnimationAndLoadSpecificSceneBuildIndex(buildIndex));
    }
    private IEnumerator WaitForAnimationAndLoadSpecificSceneBuildIndex(int buildIndex)
    {

        PlayExitSceneAnimation();

        yield return new WaitForSeconds(animationClipToPlay.averageDuration);

        SceneManager.LoadScene(buildIndex);

    }
    public void LoadSpecificSceneString(string sceneName)
    {
        StartCoroutine(WaitForAnimationAndLoadSpecificSceneBuildIndex(sceneName));
    }
    private IEnumerator WaitForAnimationAndLoadSpecificSceneBuildIndex(string sceneName)
    {

        PlayExitSceneAnimation();

        yield return new WaitForSeconds(animationClipToPlay.averageDuration);

        SceneManager.LoadScene(sceneName);

    }
}

[System.Serializable]
public class TransitionTypes
{
    [HideInInspector] public string _transitionTypeName;
    public GameObject _transitionGameObject;
}
