using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComputerInteraction : MonoBehaviour, IInteractable
{
    private Player player;
    // Bool for if the door is unlocked
    private bool isUnlocked = false;

    // Bool for what keycards the player needs to interact with the object
    public bool needRedKeycard = false;
    public bool needBlueKeycard = false;

    public GameObject door; // Reference to the door that will open when the minigame is won
    public GameObject minigame; // Reference to the minigame object
    public GameObject mainCamera; // Reference to the main camera
    public GameObject hud; // Reference to the HUD
    public GameObject playerGO; // Reference to the player game object
    public DisplayMessage displayMessage; // Reference to the DisplayMessage script

    // Called when another collider enters the trigger collider of this object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<Player>();
            player.SetIInstance(this);
        }
    }

    // Called when another collider exits the trigger collider of this object
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.ClearIInstance();
        }
    }

    public void InteractLogic()
    {
        if (!isUnlocked && CheckKeycards()) // If not unlocked and keycard requirements are met
        {
            // Activate the minigame and deactivate other elements
            minigame.SetActive(true);
            mainCamera.SetActive(false);
            hud.SetActive(false);
            // Prevents enemys from attacking the player while in the minigame
            playerGO.SetActive(false);
        }
        else
        {
            // Display messages for missing keycards
            if (needBlueKeycard && !player.hasBlueKeycard)
            {
                displayMessage.StartMessageDisplay("Blue Keycard Required");
            }
            if (needRedKeycard && !player.hasRedKeycard)
            {
                displayMessage.StartMessageDisplay("Red Keycard Required");
            }
            if (needBlueKeycard && needRedKeycard && !player.hasBlueKeycard && !player.hasRedKeycard)
            {
                displayMessage.StartMessageDisplay("Red and Blue Keycards Required");
            }
        }
    }

    // Check if the required keycards are present
    private bool CheckKeycards()
    {
        if (needBlueKeycard && !player.hasBlueKeycard)
        {
            return false;
        }
        if (needRedKeycard && !player.hasRedKeycard)
        {
            return false;
        }
        return true;
    }

    // End the minigame, unlock the interaction if the player won
    public void EndGame(bool won)
    {
        minigame.SetActive(false);
        mainCamera.SetActive(true);
        hud.SetActive(true);
        playerGO.SetActive(true);
        if (won)
        {
            isUnlocked = true;
            // disables the door tilemap so the player can walk through
            door.SetActive(false);
            // Changes the state of the game, increases difficulty
            FindObjectOfType<GameManager>().GameStateChangeEnd();
            GetComponent<Collider2D>().enabled = false; // Disable the collider so player can no longer activate the computer
        }
    }
}
