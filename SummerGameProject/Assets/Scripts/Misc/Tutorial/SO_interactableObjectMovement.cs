using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;

public class SO_interactableObjectMovement : MonoBehaviour
{
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private float desiredDuration = 3f;
    [SerializeField] private AnimationCurve curve;

    private float elapsedTime;
    private bool isLerping = false;
    private float percentageComplete;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLerping)
        {
            elapsedTime += Time.deltaTime;
            Lerping();
        }
    }

    public void StartLerping()
    {
        isLerping = true;
        elapsedTime = 0f;
    }

    private void Lerping()
    {
        percentageComplete = elapsedTime / desiredDuration;
        transform.position = Vector3.Lerp(startPosition, endPosition, curve.Evaluate(percentageComplete));

        if (percentageComplete >= 1f)
        {
            isLerping = false;
        }
    }
}
