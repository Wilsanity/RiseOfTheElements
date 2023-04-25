using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public void NewGame()
    {
        Debug.Log("Starting game");
        FindObjectOfType<ScreenFader>().fadeImage.gameObject.SetActive(true);//Start new game
    }

    public void QuitGame()
    {
        Debug.Log("Game is ended");
        Application.Quit();
    }
}
