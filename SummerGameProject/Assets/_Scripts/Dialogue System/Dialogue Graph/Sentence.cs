using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Sentence : ScriptableObject
{

    [Tooltip("A string variable representing the player or npc's name")]
    public StringVariable from;

    [TextArea (3, 10)]
    public string text = "text";

    [Tooltip("The sound file to be used for the dialogue")]
    public AudioClip audio;

    [Tooltip("Available only when it has no options")]
    public Sentence nextSentence;

    public List<Choice> options = new List<Choice>();


    public bool HasOptions()
    {
        if (options.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

[System.Serializable]
public class Choice
{
    [TextArea (3, 10)]
    public string text;
    public Sentence nextSentence;
    public GameEvent consequence;
}