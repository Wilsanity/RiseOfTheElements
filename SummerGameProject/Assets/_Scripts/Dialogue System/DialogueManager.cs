using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class DialogueManager : MonoBehaviour
{
    //The main coupling between our data information for dialogue and the showable ui.

    [Header("GameEvents")]
    public GameEvent ConversationEnded;
    public GameEvent ConversationStarted;

    [Space]
    public StringVariable playerName;

    [Tooltip("For the typing animation. Determine how long it takes for each character to appear")]
    public float timeBetweenChars = 0.05f;

    [Header("UI")]
    public TMP_Text playerNameTxtUI;
    public TMP_Text npcNameTextUI;
    public TMP_Text playerTextUI;
    public TMP_Text npcTextUI;

    [Tooltip("The part of UI that display the UI")]
    public GameObject DialogueUI;

    [Tooltip("The text UIs that display options")]
    public TMP_Text[] optionsUI;

    DialogueSO dialogue;
    Sentence currentSentence;

    bool isScrolling = false;


    public void StartDialogue(DialogueSO dialogueSO)
    {
        if(!dialogueSO.isAvailable)
        {
            return;
        }
        if (ConversationStarted != null)
        {
            ConversationStarted.Raise();
        }

        playerTextUI.text = null;
        npcTextUI.text = null;
        HideOptions();
        DialogueUI.SetActive(false);

        dialogue = dialogueSO;

        if (playerNameTxtUI != null)
        {
            playerNameTxtUI.text = playerName.Value;
        }

        currentSentence = dialogue.startingSentence;

        DisplayDialogue();


    }

    public void GoToNextSentence()
    {
        //Kill coroutines.

        //If we are scrolling, end scrolling & instantly populate text field.
        if (isScrolling)
        {
            StopAllCoroutines();
            loadText();
            isScrolling = false;
            return;
        }

        //If last sentence, end dialogue


        if (currentSentence.nextSentence == null && currentSentence.HasOptions())
        {
            EndDialogue();
            return;
        }



        

        currentSentence = currentSentence.nextSentence;
        DisplayDialogue();


    }

    public void DisplayDialogue()
    {
        if (currentSentence == null)
        {
            EndDialogue();
            return;
        }

        if (!currentSentence.HasOptions())
        {
            DialogueUI.SetActive(true);
            HideOptions();


            TMP_Text dialogueText;

            if (currentSentence.from.Value == playerName.Value)
            {

                if (playerNameTxtUI != null)
                {
                    playerNameTxtUI.text = playerName.Value;
                }
                dialogueText = playerTextUI;
            }
            else
            {
                if (npcNameTextUI != null)
                {
                    npcNameTextUI.text = currentSentence.from.Value;
                }
                dialogueText = npcTextUI;
            }

            StopAllCoroutines();
            StartCoroutine(Typeout(currentSentence.text, dialogueText));

            //currentSentence.audio.LoadAudioData();
            
            


        }
        else
        {
            DisplayOptions();
        }
    }
    private void loadText()
    {
        npcTextUI.text = currentSentence.text;
    }
    IEnumerator Typeout(string sentence, TMP_Text textbox)
    {
        textbox.text = "";



        foreach (var letter in sentence.ToCharArray())
        {
            isScrolling = true;
            textbox.text += letter;

            yield return new WaitForSeconds(timeBetweenChars);

            

        }
        isScrolling = false;
        //Auto move on feature...
        
        //GoToNextSentence();



    }

    public void OptionsOnClick(int index)
    {
        Choice option = currentSentence.options[index];
        if (option.consequence != null)
        {
            Debug.Log("Raise Events");
            option.consequence.Raise();
        }
        currentSentence = option.nextSentence;

        DisplayDialogue();
    }

    public void DisplayOptions()
    {
        //This is bad...
        //We want our dialogue to display the options...
        //DialogueUI.SetActive(false);

        if (currentSentence.from.Value == playerName.Value)
        {

            if (playerNameTxtUI != null)
            {
                playerNameTxtUI.text = playerName.Value;
            }
        }

        if (currentSentence.options.Count <= optionsUI.Length)
        {
            for (int i = 0; i < currentSentence.options.Count ; i++)
            {
                Debug.Log(currentSentence.options[i].text);
                optionsUI[i].text = currentSentence.options[i].text;
                optionsUI[i].gameObject.SetActive(true);
            }
        }
    }

    public void HideOptions()
    {
        foreach (TMP_Text option in optionsUI)
        {
            option.gameObject.SetActive(false);
        }
    }

    public void EndDialogue()
    {
        Debug.Log("Dialogue Ended");
        DialogueUI.SetActive(false);

        if (ConversationEnded != null)
        {
            ConversationEnded.Raise();
        }

    }

}
