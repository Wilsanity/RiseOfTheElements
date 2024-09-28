using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomButtonHitBox : MonoBehaviour
{
    [SerializeField]
    private Image buttonImageComponent;
    // Start is called before the first frame update
    void Start()
    {
        if (buttonImageComponent != null)
        {
            buttonImageComponent.alphaHitTestMinimumThreshold = 0.1f;
        }
    }
}
