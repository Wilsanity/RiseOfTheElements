using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WorldData.cs - 
/// Holds Global Information.
/// </summary>
public class WorldData : MonoBehaviour
{
    static public WorldData Instance = null;

    public float worldTimer = 0;

    void Awake()
    {
        // Check if there is already an instance of the world state.
        if (Instance == null)
            Instance = this;
        else if (Instance != this)  // Destroy if not instance
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void Update()
    {
        worldTimer += Time.deltaTime;
    }
}
