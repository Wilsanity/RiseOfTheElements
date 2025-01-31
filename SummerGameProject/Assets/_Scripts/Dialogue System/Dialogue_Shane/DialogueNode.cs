using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNode
{
    public string DialogueText { get; set; }
    public List<Choice> Choices { get; set; }

    public DialogueNode(string dialogueText)
    {
        DialogueText = dialogueText;
        Choices = new List<Choice>();
    }
}

public class Choice
{
    public string ChoiceText { get; set; }
    public DialogueNode NextNode { get; set; }

    public Choice(string choiceText, DialogueNode nextNode)
    {
        ChoiceText = choiceText;
        NextNode = nextNode;
    }
}
