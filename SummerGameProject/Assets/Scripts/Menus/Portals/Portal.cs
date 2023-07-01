using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Linq;

public class Portal : Interactable
{
    bool portalActive;
    
    private void Start()
    {
        InitPortal();
    }

    public void InitPortal()
    {
        if (portalActive) return;
        portalActive = true;
        
        string filePath = Application.dataPath + "/test.txt";
        List<string> portals = File.ReadAllLines(filePath).ToList();

        string currentPortalID = SceneManager.GetActiveScene().name + "_" + gameObject.name;

        if (!portals.Contains(currentPortalID))
        {
            portals.Add(currentPortalID);
            File.WriteAllLines(filePath, portals.ToArray());
        }

        //Enable UI to choose a portal to telleport to TODO:
    }

    public void ActivatePortal(string portalID)
    {
        string sceneName = portalID.Split('_')[0];
        string portalName = portalID.Split('_')[1];

        PlayerPrefs.SetInt("isPortalUsed", 1);
        PlayerPrefs.SetString("currentPortal", portalName);

        SceneManager.LoadScene(sceneName);
    }
}
