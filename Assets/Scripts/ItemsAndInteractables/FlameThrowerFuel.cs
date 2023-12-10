using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerFuel : MonoBehaviour, IInteractable
{
    private Player player;
    public int fuelAmount = 25;

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

    // Adds fuelAmount to players current fuel then destroys the gameobject
    public void InteractLogic()
    {
        player.flameStockPile += fuelAmount;
        player.UpdateAmmoCounter();
        Destroy(this.gameObject);
    }
}
