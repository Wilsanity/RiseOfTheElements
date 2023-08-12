using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    //specific scenes for loading.
    #region
    [SerializeField]
    private Object creditsScene;
    [SerializeField]
    private Object levelOneScene;
    #endregion

    [SerializeField] CanvasGroup startMenu;
    [SerializeField] CanvasGroup loadGameMenu;
    [SerializeField] CanvasGroup optionsMenu;

    [SerializeField] LoadMenuManager loadManager;

    #region
    public void start_New_Game()
    {
        //TO DO...

        // create new save file

        //play book animation for new game


        if (levelOneScene != null)
        {
            SceneManager.LoadScene(levelOneScene.name);
        }
        else
        {
            Debug.Log("Unable to load game. check to make sure the MainMenuManager component has refrence to the game's first scene");
        }
     
    }
    public void continue_Last_Game()
    {
        Object lastLocation = null;

        //check first save data in the array
        //lastLocation = the scene where the player saved.

        if (lastLocation != null)
        {
            SceneManager.LoadScene(lastLocation.name);
        }
        else
        {
            Debug.Log("Unable to load Last Save");
            Debug.Log("*Feature still in development*");
        }
    }
    public void open_Load_Game_Menu()
    {
        //play book animation *flip to load page*

        //disable start screen
        startMenu.alpha = 0;
        startMenu.interactable = false;
        startMenu.blocksRaycasts = false;

        ///call fill function from load manager


        //enable load game menu
        loadGameMenu.alpha = 1;
        loadGameMenu.interactable = true;
        loadGameMenu.blocksRaycasts = true;
    }
    public void load_Game(PlayerSaveFileData saveData)
    {
        Object lastLocation = null;

        if (lastLocation != null)
        {
            SceneManager.LoadScene(lastLocation.name);
        }
        else
        {
            Debug.Log("Unable to load Save");
            Debug.Log("*Feature still in development*");
        }
    }
    public void open_Options_Menu()
    {
        startMenu.alpha = 0;
        startMenu.interactable = false;
        startMenu.blocksRaycasts = false;

        //play book animation
        //wait for book animaton to enable options *hide transition*

        optionsMenu.alpha = 1;
        optionsMenu.interactable = true;
        optionsMenu.blocksRaycasts = true;
    }

    public void backToStartMenu()
    {
        if (loadGameMenu != null && loadGameMenu.alpha > 0)
        {
            loadGameMenu.alpha = 0;
            loadGameMenu.interactable = false;
            loadGameMenu.blocksRaycasts = false;
        }
        else if (optionsMenu != null && optionsMenu.alpha > 0)
        {
            optionsMenu.alpha = 0;
            optionsMenu.interactable = false;
            optionsMenu.blocksRaycasts = false;
        }
        startMenu.alpha = 1;
        startMenu.interactable = true;
        startMenu.blocksRaycasts = true;
    }

    public void play_Credits ()
    {
        // play credits animation...

        if (creditsScene != null)
        {
            SceneManager.LoadScene(creditsScene.name, LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Unable to load credits.");
        }        
    }
    public void exit_Game()
    {
        Application.Quit();
    }
    #endregion
}