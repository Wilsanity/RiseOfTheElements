using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideController : MonoBehaviour
{
    public Transform orbitCenter; // The object to orbit around
    public float orbitSpeed = 15f; // The normal speed of the orbit
    public float boostedOrbitSpeed = 45f; // The boosted speed of the orbit
    public float speedChangeDelay = 5f; // The time in seconds between speed changes
    public float maxRadius = 3f;

    private bool isBoosted = false;
    private float timer = 0f;

    void Update()
    {
        

        Vector3 centerToGuide = transform.position - orbitCenter.position;
        float currentDist = centerToGuide.magnitude;

        //If the guide moves beyond the maxRadius we get a new position within
        //the maxRadius to keep it within the allowed distance from the orbit center.
        if (currentDist > maxRadius)
        {
            //Get a position within the maxRadius
            Vector3 targetPosition = orbitCenter.position + centerToGuide.normalized * maxRadius;
            transform.position = targetPosition;
        }

        transform.LookAt(orbitCenter);//Always look at the player in a fixed rotation pattern.

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
