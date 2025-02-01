using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//This should be a singleton, or some form of that.
public class DialogueManagerOLD : MonoBehaviour
{
    private DialogueNodeOLD currentNode;

    //Quick slam of a ui Element
    //Linking ui is ok I think given it's one entire prefab. 
    //However, removing the choices is good, since we want to 
    public GameObject ui;
    public TMP_Text myText;
    public TMP_Text[] choices;


    public void StartDialogue(DialogueNodeOLD startingNode)
    {

        //Here either some reference to game manager instance or other solution.
        //This is to handle swapping input contexts & disabling player controls to ui.
        //For now we will use player controller script.
        FindObjectOfType<PlayerController>().swapInputContext("PlayerUI");

        currentNode = startingNode;
        DisplayCurrentDialogue();

    }   
    
    public void DisplayCurrentDialogue()
    {
        ui.SetActive(true);
        Debug.Log(currentNode.DialogueText);
        myText.text = currentNode.DialogueText;


        
        //Here we should use our choices count to determine the amount of choices we have ?



        for (int i = 0; i < currentNode.Choices.Count; i++)
        {
            Debug.Log($" {i + 1}: {currentNode.Choices[i].ChoiceText}");
            


            //This is deprecated, needs to be different (not dragged in)
            choices[i].text = currentNode.Choices[i].ChoiceText;


        }
    }

    public void SelectChoice(int choiceIndex)
    {
        if (choiceIndex < 0 || choiceIndex >= currentNode.Choices.Count)
        {
            Debug.LogError("Invalid Choice Index");
            return;
        }

        currentNode = currentNode.Choices[choiceIndex].NextNode;
        DisplayCurrentDialogue();
    }

    public void EndDialogue()
    {
        ui.SetActive(false);
    }


}
