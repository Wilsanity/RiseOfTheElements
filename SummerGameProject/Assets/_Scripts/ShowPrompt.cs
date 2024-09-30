using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPrompt : MonoBehaviour
{
    [SerializeField]
    TutorialUIManager promptManager;

    [SerializeField]
    string inputPrompt = "";

    private void OnCollisionEnter(Collision collision)
    {
        promptManager.promptPlayer(inputPrompt);
    }
}
