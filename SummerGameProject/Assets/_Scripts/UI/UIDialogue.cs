using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIDialogue : UIElement
{
    private List<Dialogue> dialogueQueue = new List<Dialogue>();
    private int sentenceIndex = 0;
    
    
    public TMP_Text displayText = null;

    void Start()
    {
        
        //displayText = this.gameObject.GetComponentInChildren<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    //private void displayElements();

    public override void setVisibility(bool visible)
    {
        isVisible = visible;

        //When we are visible we need to make sure our ui is loaded. Always load the first in the queue.

        Debug.Log(dialogueQueue.Count);

        //Stops issue with index array
        if (dialogueQueue.Count > 0)
        {
            displayText.text = dialogueQueue[0].sentences[sentenceIndex];
        }

        //Clear our queue when dialogue UI will be exited, clearing any unfinished dialogue. & Reset our sentence index.
        if (visible == false)
        {
            dialogueQueue.Clear();
            sentenceIndex = 0;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().swapInputContext("Player");
        }
        this.gameObject.SetActive(isVisible);
        //Awkward but we need to call our player class when we disable ourself..


        
    }

    public override void interactUI()
    {
        if (progressChat())
        {
            setVisibility(false);
        }
        else
        {
            //Check if there is anything in our buffer
            if (dialogueQueue.Count > 0)
            {
                displayText.text = dialogueQueue[0].sentences[sentenceIndex];
                //return;
            }
            else
            {
                setVisibility(false);
            }
        }
    }



    //I genuinely think it makes more sense for the NPC to directly call this function from this class...
    //However it's currently set up to override our abstracted function from the UIElement class.
    public override void sendData(Dialogue dialogue)
    {
        dialogueQueue.Add(dialogue);
        Debug.Log("Dialogue added");
    }




    //Progress our sentence from our current dialogue, if dialogue has no more sentences, check if our buffer has another 
    private bool progressChat()
    {
        sentenceIndex += 1;

        if (dialogueQueue[0].sentences.Length == sentenceIndex)
        {
            //Dequeue our dialogue as it's finished & exit function
            //I should also clear our text.
            Debug.Log("Remove being called");
            displayText.text = "";
            dialogueQueue.RemoveAt(0);
            sentenceIndex = 0;
            return true;
        }
        displayText.text = dialogueQueue[0].sentences[sentenceIndex];
        return false;
        
    }

}
