using UnityEngine;
using Fungus;

public class NPCInteraction : MonoBehaviour
{
    public bool canInteract;
    public string Name = "Name";
    
    public Flowchart flowchart;

    public void Interact_NPC()
    {
        if (!canInteract) return;
        switch (Name)
        {
            case "Doll" :
                flowchart.ExecuteBlock("Doll_I_I");
                break;
            default:
                Debug.Log("No thing");
                break;
        }
    }
    
    public void switchState()
    {
        if (canInteract)
        {
            canInteract = false;
            return;
        }
        else
        {
            canInteract = true;
        }
    }
}

