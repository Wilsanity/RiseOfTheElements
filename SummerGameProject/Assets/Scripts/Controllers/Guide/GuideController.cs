using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideController : MonoBehaviour
{
    public Transform orbitCenter; // The object to orbit around
    public float orbitSpeed = 15f; // The normal speed of the orbit
    public float boostedOrbitSpeed = 45f; // The boosted speed of the orbit
    public float speedChangeDelay = 5f; // The time in seconds between speed changes

    private bool isBoosted = false;
    private float timer = 0f;

    void Update()
    {
        transform.RotateAround(orbitCenter.position, Vector3.up, orbitSpeed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= speedChangeDelay)
        {
            timer = 0f;
            isBoosted = !isBoosted;
            orbitSpeed = isBoosted ? boostedOrbitSpeed : 15f;
        }
    }
}
