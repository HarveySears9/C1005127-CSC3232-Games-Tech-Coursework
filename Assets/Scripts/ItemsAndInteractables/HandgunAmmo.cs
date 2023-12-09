using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandgunAmmo : MonoBehaviour, IInteractable
{
    private Player player;
    public int ammoAmount = 10;

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

    // Adds ammoAmount to players current ammo then destroys the gameobject
    public void InteractLogic()
    {
        player.handgunAmmoStockPile += ammoAmount;
        player.UpdateAmmoCounter();
        Destroy(this.gameObject);
    }
}
