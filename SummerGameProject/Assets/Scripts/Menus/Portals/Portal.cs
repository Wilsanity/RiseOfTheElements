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

    // variables used in optional portal functionality
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
        // unlocks the portal for use by the player at other portals
        UnlockPortal();

        // if optional first use action is selected, do not open portal menu and finish actions
        if (firstUseActions != FirstUseActions.None && firstActionReady)
        {
            firstActionReady = false;
            return;
        }

        // if portal is not on cooldown, start portal UI actions
        if (!initOnCooldown) StartCoroutine(PortalInitCoolDown());

        IEnumerator PortalInitCoolDown()
        {
            // start cooldown on portal UI
            initOnCooldown = true;

            // Load additive scene for portal UI
            SceneManager.LoadSceneAsync("Portal UI", LoadSceneMode.Additive);

            // wait 1 second and end the cooldown for use
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

// world types to sort portals by levels
public enum WorldType
{
    Earth,
    Water,
    Fire,
    Air,
    Debug
}

// stores optional function enum ids
public enum FirstUseActions
{
    None,
    TeleportPlayer,
}