using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This should be a singleton, or some form of that.
public class DialogueManager : MonoBehaviour
{
    private DialogueNode currentNode;

    public void StartDialogue(DialogueNode startingNode)
    {
        currentNode = startingNode;
        DisplayCurrentDialogue();

    }   
    
    public void DisplayCurrentDialogue()
    {
        Debug.Log(currentNode.DialogueText);

        for (int i = 0; i < currentNode.Choices.Count; i++)
        {
            Debug.Log($" {i + 1}: {currentNode.Choices[i].ChoiceText}");

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
}
