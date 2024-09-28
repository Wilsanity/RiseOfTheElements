using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideController : MonoBehaviour
{
    [SerializeField] Transform orbitCenter; // The object to orbit around
    public float baseOrbitSpeed = 15f; // The normal speed of the orbit
    public float activeOrbitSpeed = 15f; // The current active speed of the orbit
    public float boostedOrbitSpeed = 45f; // The boosted speed of the orbit
    public float speedChangeDelay = 5f; // The time in seconds between speed changes
    public float maxRadius = 3f;
    public float minRadius = 0.5f;

    private bool isBoosted = false;
    private float timer = 0f;

    void Update()
    {
        

        Vector3 centerToGuide = transform.position - orbitCenter.position;
        
        float currentDist = centerToGuide.magnitude;

        //If the guide moves beyond the maxRadius we get a new position within
        //the maxRadius to keep it within the allowed distance from the orbit center.
        Vector3 targetPosition = transform.position;
        if (currentDist > maxRadius)
        {
            //Get a position within the safe orbit Radius
            targetPosition = orbitCenter.position + centerToGuide.normalized * maxRadius;            
            transform.position = targetPosition;
        }
        else if (currentDist < minRadius)
        {
            //Get a position within the safe orbit Radius
            targetPosition = orbitCenter.position + centerToGuide.normalized * minRadius;
            transform.position = targetPosition;
        }
        // maintain guide Hight (y axis) with orbitCenter hight (y axis)
        if (transform.position.y != orbitCenter.position.y)
        {
            targetPosition = new Vector3 (targetPosition.x, orbitCenter.position.y, targetPosition.z);
            transform.position = targetPosition;
        }

        transform.LookAt(orbitCenter);//Always look at the player in a fixed rotation pattern.

        transform.RotateAround(orbitCenter.position, Vector3.up, activeOrbitSpeed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= speedChangeDelay)
        {
            timer = 0f;
            isBoosted = !isBoosted;
            activeOrbitSpeed = isBoosted ? boostedOrbitSpeed : baseOrbitSpeed;
        }
    }
}
