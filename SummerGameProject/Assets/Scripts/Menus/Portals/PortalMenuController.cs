using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cinemachine;

public class PortalMenuController : MonoBehaviour
{
    [SerializeField] GameObject portalButtonPrefab;

    #region Menu backdrop fade setting

    [Header("Menu Backdrop Fade Setting")]
    [SerializeField] float fadeTime;
    [SerializeField, Range(0, 1)] float fadedAlpha;
    [SerializeField] AnimationCurve fadeAnimation;

    #endregion

    #region portal menu setting

    //Initiates a 2D list to contain portal IDs by world type and portal index
    List<string>[] portals = new List<string>[5] { new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>() };
    [SerializeField] bool enableDebugPortals;

    #endregion

    InputAction escape;
    InputAction.CallbackContext escapeContext;

    private void Awake()
    {
        // start loading sequence
        StartCoroutine(LoadMenu());

        // maps escape button to the pause action
        escape = FindObjectOfType<PlayerInput>().actions["Pause"];

        // if pause action is performed, close menu
        escape.performed += EscapeFunction;

        // obtain portal list from saved file
        List<string> portalList = new List<string>(File.ReadAllLines(Application.dataPath + "/Save Data/Portal Data.txt"));

        //Fill the 2D list with portal IDs
        foreach (string portalID in portalList) AddPortal(portalID);

        //Create a button for each portal in the list
        for (int i = 0; i < portals.Length; i++) CreatePortalButtons(i);
    }

    private void AddPortal(string portalID)
    {
        int worldType = int.Parse(portalID.Split('_')[0]);
        int index = int.Parse(portalID.Split('_')[1]);

        portals[worldType].Add(portalID);
        if (portals[worldType][index] != portalID)
        {
            for (int i = index, j = portals[worldType].Count - 1; i < j; i++)
            {
                string temp = portals[worldType][i];
                portals[worldType][i] = portals[worldType][j];
                portals[worldType][j] = EditPortalIndex(temp, i + 1);
            }
        }
    }

    private string EditPortalIndex(string portalID, int newIndex)
    {
        string[] portalData = portalID.Split('_');
        portalData[1] = newIndex.ToString();
        return portalData[0] + "_" + portalData[1] + "_" + portalData[2] + "_" + portalData[3];
    }

    private void CreatePortalButtons(int worldType)
    {
        if (worldType == 4 && !enableDebugPortals) return;

        GameObject portalContainer = new GameObject();
        portalContainer.transform.parent = transform;
        portalContainer.name = ((WorldType)worldType).ToString() + " World Portals";

        portalContainer.transform.localPosition = new Vector3(-350 + (175 * worldType), 0, 0);

        foreach (string portalID in portals[worldType])
        {
            GameObject portalButton = Instantiate(portalButtonPrefab, portalContainer.transform);
            portalButton.GetComponentInChildren<TMP_Text>().text = portalButton.name = portalID.Split('_')[3];
            portalButton.GetComponent<Button>().onClick.AddListener(() => SelectPortal(portalID));

            float height = portalButton.GetComponent<RectTransform>().sizeDelta.y;

            float y = (-height * 0.5f * (portals[worldType].Count - 1)) + (height * (int.Parse(portalID.Split('_')[1])));
            portalButton.transform.localPosition = new Vector3(0, y, 0);
        }
    }

    // on portal button selected
    public void SelectPortal(string portalID)
    {
        // enable camera
        FindObjectOfType<CinemachineBrain>().enabled = true;
        
        // close mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // will read the selected portal ID adn identify the scene name and portal name
        string sceneName = portalID.Split('_')[2];
        string portalName = portalID.Split('_')[3];

        // will inform upon scene loading to spawn the player at a portal
        PlayerPrefs.SetInt("isPortalUsed", 1);
        PlayerPrefs.SetString("currentPortal", portalName);
        Debug.Log(portalName);

        // unload scene
        SceneManager.UnloadSceneAsync("Portal UI");

        // stop coroutines
        StopAllCoroutines();

        // reset time
        Time.timeScale = 1;
        escape.performed -= EscapeFunction;

        // load selected scene
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadMenu()
    {
        // find backdrop
        Image backdrop = transform.Find("Backdrop").GetComponent<Image>();
        backdrop.color = new Color(backdrop.color.r, backdrop.color.g, backdrop.color.b, 0.0f);
        float t = 0;

        // enable mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // disable camera controls
        FindObjectOfType<CinemachineBrain>().enabled = false;

        while (t < 1)
        {
            // over time...
            t += Time.unscaledDeltaTime * Mathf.Pow(fadeTime, -1);

            // fade backdrop in
            float alpha = Mathf.Lerp(0, 0.5f, fadeAnimation.Evaluate(t));
            backdrop.color = new Color(backdrop.color.r, backdrop.color.g, backdrop.color.b, alpha);

            // lerp time to stop smoothly
            Time.timeScale = Mathf.Lerp(1, 0, t);
            yield return new WaitForEndOfFrame();
        }
    }

    void EscapeFunction(InputAction.CallbackContext ctx)
    {
        // when pause action performed
        StopAllCoroutines();
        StartCoroutine(UnloadMenu());
    }

    IEnumerator UnloadMenu()
    {
        // remove pause listener
        escape.performed -= EscapeFunction;

        // stop load menu coroutine
        StopCoroutine(LoadMenu());

        // enable camera
        FindObjectOfType<CinemachineBrain>().enabled = true;

        // find backdrop
        Image backdrop = transform.Find("Backdrop").GetComponent<Image>();
        float currentAlpha = backdrop.color.a;
        
        float t = 0;

        // remove cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        while (t < 1)
        {
            // over time...
            t += Time.unscaledDeltaTime * Mathf.Pow(fadeTime, -1);

            // fade alpha of the backdrop
            float alpha = Mathf.Lerp(currentAlpha, 0, fadeAnimation.Evaluate(t));
            backdrop.color = new Color(backdrop.color.r, backdrop.color.g, backdrop.color.b, alpha);

            // lerp time back to 1
            Time.timeScale = Mathf.Lerp(0, 1, t);
            yield return new WaitForEndOfFrame();
        }

        // reset time
        Time.timeScale = 1;

        // unload menu scene
        SceneManager.UnloadSceneAsync("Portal UI");
    }
}
