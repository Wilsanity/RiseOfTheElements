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
        displayText.text = dialogueQueue[0].sentences[sentenceIndex];
        this.gameObject.SetActive(isVisible);

        
    }

    public override void interactUI()
    {
        //Interact could have different contexts here,
        //During Dialogue I assume we can progress (speed through dialogue), or select a response to the dialogue.

        //The selection is a bit more complex since it involves a selection process however we can add the speed through dialogue portion.
        //Need to make sure the dialogue ends once the array of sentences are completed.

        //This is where we should use class-defined function calls to implement proper procedure.


        //Here we want to 
        

        if (progressChat())
        {
            //Check if there is anything in our buffer
            if (dialogueQueue.Count > 0)
            {
                displayText.text = dialogueQueue[0].sentences[sentenceIndex];
                //return;
            }
            else
            {
                //Our dialogue is finished.
                //
                setVisibility(false);
                
                
                
            }

        }
        else
        {

        }


    }



    //I genuinely think it makes more sense for the NPC to directly call this function from this class...
    //However it's currently set up to override our abstracted function from the UIElement class.
    public override void sendData(Dialogue dialogue)
    {
        dialogueQueue.Add(dialogue);
        Debug.Log("Dialogue added");
    }



    //Return here is a bit ambiguous, it's used to tell if dialogue is completed & been removed (true) or dialogue is displaying the next sentence.
    //Used for potentially 
    private bool progressChat()
    {
        sentenceIndex += 1;
        Debug.Log("Progress chat called");
        //
        //I don't think we need a queue entirely but more of a buffer.

        //This works since our queue works as a buffer, once the sentences are complete, it gets removed, and the new dialogue item 
        //Is back at 0

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

        //Given our main check alreay happened we don't need an if statement.

        //Increase our sentenceIndex.
        displayText.text = dialogueQueue[0].sentences[sentenceIndex];
        return false;
        
    }

}
