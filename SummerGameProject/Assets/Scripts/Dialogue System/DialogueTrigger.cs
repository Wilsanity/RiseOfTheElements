using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

	public Dialogue dialogue;
    public GameObject dialogueBox;

    [SerializeField]
    private float leaveDiaglogueTime = 2f;

    private bool isTalking = false;

    private void Start()
    {
        PlayerController.onNPCInteract += PlayerController_onNPCInteract;
        DialogueManager.OnTalkingEnd += DialogueManager_OnTalkingEnd;
    }

    private void OnDestroy()
    {
        PlayerController.onNPCInteract -= PlayerController_onNPCInteract;
        DialogueManager.OnTalkingEnd -= DialogueManager_OnTalkingEnd;
    }

    private void DialogueManager_OnTalkingEnd()
    {
        isTalking = false;
        dialogueBox.SetActive(false);
    }

    private void PlayerController_onNPCInteract(bool isInteracting)
    {
        if(!isInteracting)
        {
            StartCoroutine(LeaveDialogue());
        }
        if(isInteracting &&!isTalking)
        {
            dialogueBox.SetActive(true);
            TriggerDialogue();
            isTalking = true;
        }
        else if (isTalking)
        {
            FindObjectOfType<DialogueManager>().DisplayNextSentence();
        }
    }

    public void TriggerDialogue ()
	{
		FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
	}

    private IEnumerator LeaveDialogue()
    {
        yield return new WaitForSeconds(leaveDiaglogueTime);
        FindObjectOfType<DialogueManager>().EndDialogue();
    }

}
