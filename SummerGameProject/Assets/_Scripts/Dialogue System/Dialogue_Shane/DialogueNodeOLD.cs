using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNodeOLD
{
    public string DialogueText { get; set; }
    public List<ChoiceOLD> Choices { get; set; }

    public DialogueNodeOLD(string dialogueText)
    {
        DialogueText = dialogueText;
        Choices = new List<ChoiceOLD>();
    }
}

public class ChoiceOLD
{
    public string ChoiceText { get; set; }
    public DialogueNodeOLD NextNode { get; set; }

    public ChoiceOLD(string choiceText, DialogueNodeOLD nextNode)
    {
        ChoiceText = choiceText;
        NextNode = nextNode;
    }
}
