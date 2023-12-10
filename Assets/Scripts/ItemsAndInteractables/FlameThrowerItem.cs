using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlameThrowerItem : MonoBehaviour, IInteractable
{
    private Player player;

    public DisplayMessage displayMessage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<Player>();
            player.SetIInstance(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.ClearIInstance();
        }
    }

    // Calls player.PickupFlameThrower in the Player script
    public void InteractLogic()
    {
        player.PickupFlameThrower();

        // calls StartMessageDisplay in the DisplayMessage script
        displayMessage.StartMessageDisplay("Press F to change weapons");

        // Destroys the gameobject
        Destroy(this.gameObject);
    }
}

