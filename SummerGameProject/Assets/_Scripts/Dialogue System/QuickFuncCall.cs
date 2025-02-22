using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickFuncCall : MonoBehaviour
{
    // Start is called before the first frame update

    public DialogueSO myDialogue;
    public GameObject obj;
    

    public void startConvo()
    {
        Debug.Log("Enable convo?");


        obj.GetComponent<DialogueManager>().StartDialogue(myDialogue);
    }
}
