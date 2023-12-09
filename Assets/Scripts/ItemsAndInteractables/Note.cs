using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour, IInteractable
{
    private Player player;

    public GameObject NoteScreenUI;
    public GameObject ammoCounter;

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

    // Pauses the game and shows note image to user
    public void InteractLogic()
    {
        NoteScreenUI.SetActive(true);
        Time.timeScale = 0.0f;
        InventoryScreen.isPaused = true;
        ammoCounter.SetActive(false);
    }
}
