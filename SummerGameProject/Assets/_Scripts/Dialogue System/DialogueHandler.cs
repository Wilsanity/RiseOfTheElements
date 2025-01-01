using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueHandler : MonoBehaviour
{
    // Start is called before the first frame update
    //This class' job is to receive data about sentences (Dialogue class) and send that data to the player ui. 
    //Used to decouple NPC from the player UI and have this class as the Observer.


    public TMP_Text text;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        

    }


    public void SendDialogue(Dialogue info)
    {
        //This will send Dialogue to the player...
        //Note in actuality it should send it to a dialogue like printer maybe?

        text.text = info.name + "\n" + info.sentences[0];

        
    }
}
