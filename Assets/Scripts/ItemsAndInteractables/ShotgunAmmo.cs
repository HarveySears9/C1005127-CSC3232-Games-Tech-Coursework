using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunAmmo : MonoBehaviour, IInteractable
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

    // Adds ammoAmount to players current ammo then destroys the game object 
    public void InteractLogic()
    {
        player.shotgunAmmoStockPile += ammoAmount;
        player.UpdateAmmoCounter();
        Destroy(this.gameObject);
    }
}
