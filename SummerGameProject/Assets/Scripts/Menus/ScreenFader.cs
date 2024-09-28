using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;//These are for creating the fade affect during certain parts of the menu
    public float fadeDuration = 1f;

    private void Start()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.canvasRenderer.SetAlpha(1f);
        fadeImage.CrossFadeAlpha(0f, fadeDuration, false);
        Invoke("StartNewGame", fadeDuration);
    }

    private void StartNewGame()
    {
        SceneManager.LoadScene("Earth Tutorial");
    }
}
