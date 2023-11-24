using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Linq;
using System.ComponentModel;
using UnityEngine.SearchService;
using Cinemachine;

public class Portal : Interactable
{
    bool initOnCooldown;

    #region FirstUseActionVariables
    [SerializeField] public FirstUseActions firstUseActions;
    private bool firstActionReady;

    [SerializeField] public string FA_SceneName;
    [SerializeField] public string FA_WaypointName;
    #endregion

    public string SaveDataPath
    {
        // will return the file directory of the protal save data text file
        get { return Application.dataPath + "/Save Data/Portal Data.txt"; }
    }

    public string PortalID
    {
        // will return the portal ID
        get { return (int)worldType + "_" + portalIndex + "_" + SceneManager.GetActiveScene().name + "_" + gameObject.name; }
    }
    
    public WorldType worldType;
    public int portalIndex = 0;

    public void InitPortal()
    {
        UnlockPortal();

        if (firstUseActions != FirstUseActions.None && firstActionReady)
        {
            firstActionReady = false;
            return;
        }

        if (!initOnCooldown) StartCoroutine(PortalInitCoolDown());

        IEnumerator PortalInitCoolDown()
        {
            initOnCooldown = true;
            SceneManager.LoadSceneAsync("Portal UI", LoadSceneMode.Additive);
            yield return new WaitForSeconds(1);

            initOnCooldown = false;
        }
    }

    public void UnlockPortal()
    {
        // will create a list from the portal data text file
        List<string> portals = File.ReadAllLines(SaveDataPath).ToList();

        if (!portals.Contains(PortalID))
        {
            // if the portal ID is not in the list, add it to the list and write the new list to the portal data text file
            portals.Add(PortalID);
            File.WriteAllLines(SaveDataPath, portals.ToArray());
            Debug.Log("Portal has been unlocked!");

            #region

            switch (firstUseActions)
            {
                case FirstUseActions.None: break;
                case FirstUseActions.TeleportPlayer:
                    PlayerPrefs.SetInt("isPortalUsed", 1);
                    PlayerPrefs.SetString("currentPortal", FA_WaypointName);
                    StopAllCoroutines();
                    Time.timeScale = 1;
                    SceneManager.LoadScene(FA_SceneName);
                    firstActionReady = true;
                    break;
                default:
                    Debug.LogError("First Use Action Not Recognized!");
                    break;
            }

            #endregion
        }
        else
        {
            Debug.Log("Portal is already unlocked!");
        }
    }

    public void LockPortal()
    {
        // will create a list from the portal data text file
        List<string> portals = File.ReadAllLines(SaveDataPath).ToList();

        if (portals.Contains(PortalID))
        {
            // if the portal ID is in the list, remove it from the list and write the new list to the portal data text file
            portals.Remove(PortalID);
            File.WriteAllLines(SaveDataPath, portals.ToArray());
            Debug.Log("Portal has been locked!");
        }
        else
        {
            Debug.Log("Portal is already locked!");
        }
    }

    public void ClearPortalData()
    {
        // will clear the portal data text file
        File.WriteAllLines(SaveDataPath, new string[0]);
        Debug.Log("Portal has been Cleared!");
    }
}

public enum WorldType
{
    Earth,
    Water,
    Fire,
    Air,
    Debug
}

public enum FirstUseActions
{
    None,
    TeleportPlayer,
}