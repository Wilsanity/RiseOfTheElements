using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //This class is used to manage player UI...
    //I think having multiple ui elements might be necessary however. It needs to be abstract
    //Example, DialogueUI, InventoryUI will call same functions however operate differently...
    //This class will just call those same functions not caring about the implementation.

    //Holder for our ui elements should be a prefab list of pre-created UI.
    public List<UIElement> uIElements;

    private bool uiEnabled = false;


    // Start is called before the first frame update
    void Start()
    {
        if (uIElements == null)
        {   
            Debug.LogWarning("UI Manager has no stored UI Elements." + "\n " + "UIManager.cs");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (uiEnabled)
        {
            //
            //
        }
    }

    public void disableUI()
    {
        foreach (UIElement element in uIElements)
        {
            
            if (element.getVisibility())
            {
                //Return the first true this could be bad
                element.setVisibility(false);
                break;
            }
        }
    }

    public void enableUI(int uiIndex)
    {
        Debug.Log("Enable UI Called in UI Manager");

        //Debug.Log("uiIndex val = " + uiIndex);
        //Debug.Log("uiElements size = " + uIElements.Count);
        uIElements[uiIndex].setVisibility(true);
    }




    public void interact()
    {
        //Again find active UI then
        UIElement element = getEnabledElement();

        //Stop interaction if nothing enabled...
        if (!element)
        {
            return;
        }
        element.interactUI();
    }

    private UIElement getEnabledElement()
    {

        foreach (UIElement element in uIElements)
        {

            if (element.getVisibility())
            {
                return element;
            }
        }
        return null;
    }


    //This wouldn't work because of how UIElements are setup 
    public void sendData(Dialogue dialogue)
    {


        //This is our error, it sends data to the specific enabled element however rather
        //I'll keep this for now however comment it out.
        if (!getEnabledElement())
        {
            //return;
        }

        uIElements[0].sendData(dialogue);

        //Idk if i should use this or use the proper index.
        //getEnabledElement().sendData(dialogue);
    }

}
