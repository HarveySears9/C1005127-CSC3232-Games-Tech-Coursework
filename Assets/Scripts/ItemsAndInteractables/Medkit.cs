using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : MonoBehaviour, IInteractable
{
    private Player player;

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

    // Increments the amount of medkits the player has
    public void InteractLogic()
    {
        if(player.currentMedkits < player.maxMedkits)
        {
            player.currentMedkits++;
            Destroy(this.gameObject);
        }
    }
}
