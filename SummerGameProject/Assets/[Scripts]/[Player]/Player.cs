using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("i"))
        {
            inventory.OpenInventory();

            //Will be done in states later on
            MouseLook temp = FindObjectOfType<MouseLook>();
            temp.enabled = !temp.enabled;
        }
    }

    public void PickUpItem(int id)
    {
        inventory.AddItem(id);
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKey("f"))
        {
            if (other.tag.Equals("NPC"))
            {
                other.GetComponent<DialogueTrigger>().TriggerDialogue();
            }
        }
    }
}
