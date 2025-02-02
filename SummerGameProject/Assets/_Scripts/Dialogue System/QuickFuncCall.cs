using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickFuncCall : MonoBehaviour
{
    // Start is called before the first frame update

    public DialogueSO myDialogue;
    public GameObject obj;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startConvo()
    {
        Debug.Log("Enable convo?");

        FindObjectOfType<PlayerController>().swapInputContext("PlayerUI");


        obj.GetComponent<DialogueManager>().StartDialogue(myDialogue);
    }
}
