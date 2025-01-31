using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerDialogueSample : MonoBehaviour
{
    public DialogueManager myManager;
    private DialogueNode myStartNode;

    private void Start()
    {

        DialogueNode node1 = new DialogueNode("Hello I'm a farmer! How can I help you today?");
        DialogueNode node2 = new DialogueNode("I have a quest for you.");
        DialogueNode node3 = new DialogueNode("Goodbye, safe travels");


        node1.Choices.Add(new Choice("Ask about a quest", node2));
        node1.Choices.Add(new Choice("Say goodbye", node3));

        node2.Choices.Add(new Choice("Accept Quest", node3));
        node2.Choices.Add(new Choice("Decline Quest", node1));

        myStartNode = node1;



    }

    public void startDialogue()
    {
        myManager.StartDialogue(myStartNode);
    }


}
