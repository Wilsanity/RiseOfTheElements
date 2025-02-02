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
    private void Start()
    {
        IEnumerator EnableSkipTimer()
        {
            SkipEnabled(false);
            yield return new WaitForSeconds(5);
            SkipEnabled(true);
        }
        StartCoroutine(EnableSkipTimer());

        skipCredits = playerInput.actions["Pause"];
        skipCredits.performed += load_MainMenu;
    }

    void load_MainMenu(InputAction.CallbackContext ctx)
    {
        Debug.Log(skipEnable);
        if (skipEnable)
        {
            Debug.Log("Loading Main Menu");
            skipCredits.performed -= load_MainMenu;
            SceneManager.LoadScene(mainMenuScene.name, LoadSceneMode.Single);
        }
    }
    
    void load_MainMenuFromAnimation()
    {
        skipCredits.performed -= load_MainMenu;
        SceneManager.LoadScene(mainMenuScene.name, LoadSceneMode.Single);
    }

    void SkipEnabled(bool enable)
    {
        skipEnable = enable;
        Debug.Log(skipEnable);
    }
}
