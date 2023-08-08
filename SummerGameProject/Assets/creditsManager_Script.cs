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

    bool skipEnable;
    private void Awake()
    {
        skipCredits = playerInput.actions["Pause"];
        skipEnable = false;
    }
    private void OnEnable()
    {
        skipCredits.performed += ctx => load_MainMenu();
    }
    void load_MainMenu ()
    {
        if (skipEnable)
        {
            SceneManager.LoadScene(mainMenuScene.name);
        }
    }

    void enableSkip()
    {
        skipEnable = true;
    }
}
