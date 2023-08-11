using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class creditsManager_Script : MonoBehaviour
{
    [SerializeField]
    private Object mainMenuScene;
    InputAction skipCredits;
    [SerializeField]
    PlayerInput playerInput;

    public bool skipEnable;
    private void Awake()
    {
        skipCredits = playerInput.actions["Pause"];
    }
    private void OnEnable()
    {
        skipCredits.performed += ctx => load_MainMenu();
    }
    void load_MainMenu ()
    {
        Debug.Log(skipEnable);
        if (skipEnable)
        {
            Debug.Log(skipEnable);
            SceneManager.LoadScene(mainMenuScene.name);
        }
    }

    void enableSkip()
    {
        Debug.Log(skipEnable);
        skipEnable = true;
        Debug.Log(skipEnable);
    }
    void disableSkip()
    {
        Debug.Log(skipEnable);
        skipEnable = false;
        Debug.Log(skipEnable);
    }
}
