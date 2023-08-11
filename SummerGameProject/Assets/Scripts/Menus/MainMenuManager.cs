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
        //read save data array and compile data into the save data form
        //add completed forms to the safe file scroll box
        //enable load game menu
    }
    public void load_Game(Object saveData)
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

    }
    public void play_Credits ()
    {
        // play credits animation...

        if (creditsScene != null)
        {
            SceneManager.LoadScene(creditsScene.name);
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
