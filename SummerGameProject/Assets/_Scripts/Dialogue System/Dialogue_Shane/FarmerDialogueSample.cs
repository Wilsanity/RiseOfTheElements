using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerDialogueSample : MonoBehaviour
{
    public DialogueManagerOLD myManager;
    private DialogueNodeOLD myStartNode;

    private void Start()
    {

        DialogueNodeOLD node1 = new DialogueNodeOLD("Hello I'm a farmer! How can I help you today?");
        DialogueNodeOLD node2 = new DialogueNodeOLD("I have a quest for you.");
        DialogueNodeOLD node3 = new DialogueNodeOLD("Goodbye, safe travels");


        node1.Choices.Add(new ChoiceOLD("Ask about a quest", node2));
        node1.Choices.Add(new ChoiceOLD("Say goodbye", node3));

        node2.Choices.Add(new ChoiceOLD("Accept Quest", node3));
        node2.Choices.Add(new ChoiceOLD("Decline Quest", node1));

        myStartNode = node1;



    }

    public void startDialogue()
    {
        myManager.StartDialogue(myStartNode);
    }


}
