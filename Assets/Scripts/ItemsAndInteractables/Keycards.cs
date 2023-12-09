using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycards : MonoBehaviour, IInteractable
{
    private Player player;
    public bool isBlue;

    GameManager gameManager;

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

    // If keycard is blue, sets player.hasBlueKeycard to true
    // else then keycard must be red so sets player.hasRedKeycard to true
    public void InteractLogic()
    {
        if (isBlue)
        {
            player.hasBlueKeycard = true;
        }
        else
        {
            player.hasRedKeycard = true;
        }

        if(player.hasBlueKeycard && !player.hasRedKeycard)
        {
            FindObjectOfType<GameManager>().GameStateChangeOneKeycard();
        }else if(!player.hasBlueKeycard && player.hasRedKeycard)
        {
            FindObjectOfType<GameManager>().GameStateChangeOneKeycard();
        }

        Destroy(this.gameObject);
    }
}
