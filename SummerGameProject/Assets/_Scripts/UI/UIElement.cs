using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIElement : MonoBehaviour
{
    // Start is called before the first frame update
    //Used as a base class for UI, implementation should be in the children members.




    protected bool isVisible = false;
    private int amountElements = 0;

    public bool getVisibility() { return isVisible; }
    public abstract void setVisibility(bool visible);

    //This is function needs to be implemented by base classes
    public abstract void interactUI();

    //I could use a multiple overridden function but these would be fully implemented... which sucks <- We go with this one for now...

    public virtual void sendData(Dialogue dialogue) { }
    public virtual void sendData() { }

    //public abstract void sendData();

    //Either we do above or have the UIManager try to cast to the UIDialogue class 

    //I want a way to have one function which can be overidden ?




    // Update is called once per frame
}
