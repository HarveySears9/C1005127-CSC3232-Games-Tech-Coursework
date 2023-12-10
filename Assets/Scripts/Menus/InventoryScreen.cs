using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryScreen : MonoBehaviour
{
    // Static variable to track whether the inventory is paused
    public static bool isPaused = false;

    // UI for inventory screen
    public GameObject inventoryScreenUI;
    public GameObject inventoryMainScreenUI;
    public GameObject medkitScreenUI;
    public GameObject ammoScreenUI;
    public GameObject shotgunAmmoScreenUI;
    public GameObject flameFuelScreenUI;
    public GameObject blueKeycardScreenUI;
    public GameObject redKeycardScreenUI;

    public GameObject pauseMenuScreenUI;
    public GameObject controlsScreenUI;

    public GameObject displayMessage;

    // UI for note screen (used by the note item)
    public GameObject NoteScreenUI;

    // UI for the ammoCounter
    public GameObject ammoCounter;

    // Reference to the Player script
    public Player player;

    // UI Text
    [SerializeField] private TextMeshProUGUI medkitText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI shotgunAmmoText;
    [SerializeField] private TextMeshProUGUI flameFuelText;
    [SerializeField] private TextMeshProUGUI blueKeycardText;
    [SerializeField] private TextMeshProUGUI redKeycardText;

    // UI Icons for items that can be picked up
    [SerializeField] private GameObject blueKeycardIcon;
    [SerializeField] private GameObject redKeycardIcon;
    [SerializeField] private GameObject shotgunIcon;
    [SerializeField] private GameObject flameFuelIcon;

    // UI Text for sub menus
    [SerializeField] private TextMeshProUGUI medkitScreenText;
    [SerializeField] private TextMeshProUGUI ammoScreenText;
    [SerializeField] private TextMeshProUGUI shotgunAmmoScreenText;
    [SerializeField] private TextMeshProUGUI flameFuelScreenText;

    // Update is called once per frame
    void Update()
    {
        // Press tab to toggle game between being paused and unpaused
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // Method to resume the game
    void Resume()
    {
        // Set main inventory screen to active and sets all other menus to inactive
        inventoryMainScreenUI.SetActive(true);
        medkitScreenUI.SetActive(false);
        ammoScreenUI.SetActive(false);
        shotgunAmmoScreenUI.SetActive(false);
        flameFuelScreenUI.SetActive(false);
        blueKeycardScreenUI.SetActive(false);
        redKeycardScreenUI.SetActive(false);

        controlsScreenUI.SetActive(false);
        pauseMenuScreenUI.SetActive(false);
        displayMessage.SetActive(true);

        // Deactivate the inventory and resume the timescale
        inventoryScreenUI.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;

        // Activate the ammo counter and hide the Note screen
        ammoCounter.SetActive(true);
        NoteScreenUI.SetActive(false);
    }

    // Pause the game
    void Pause()
    {
        inventoryScreenUI.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;

        // Deactivate the ammo counter and set up the inventory screen
        ammoCounter.SetActive(false);
        displayMessage.SetActive(false);
        SetInventoryScreen();
    }

    // Set up the inventory screen with current player inventory information
    void SetInventoryScreen()
    {
        // Update the displayed text and icons for different inventory items
        ammoText.text = "Handgun Ammo: " +
            player.currentHandgunAmmo + "/" + player.handgunAmmoStockPile;
        ammoScreenText.text = "Handgun Ammo: " +
            (player.currentHandgunAmmo + player.handgunAmmoStockPile);
        medkitText.text = "Medkits: " +
            player.currentMedkits + "/" + player.maxMedkits;
        medkitScreenText.text = "Medkits: " +
            player.currentMedkits + "/" + player.maxMedkits;

        // Display or hide the blue keycard icon based on player possession
        blueKeycardIcon.SetActive(player.hasBlueKeycard);

        // Display or hide the red keycard icon based on player possession
        redKeycardIcon.SetActive(player.hasRedKeycard);

        // Display or hide the shotgun icon and update shotgun ammo text
        if (player.hasShotgun)
        {
            shotgunIcon.SetActive(true);
            shotgunAmmoText.text = "Shotgun Ammo: " +
            player.currentShotgunAmmo + "/" + player.shotgunAmmoStockPile;
            shotgunAmmoScreenText.text = "Shotgun Ammo: " +
                (player.currentShotgunAmmo + player.shotgunAmmoStockPile);
        }
        else { shotgunIcon.SetActive(false); }
        // Display or hide the flame fuel icon and update shotgun ammo text
        if (player.hasFlameThrower)
        {
            flameFuelIcon.SetActive(true);
            flameFuelText.text = "Flame Fuel: " +
            (int)player.currentFlame + "/" + player.flameStockPile;
            flameFuelScreenText.text = "Flame Fuel: " +
                ((int)player.currentFlame + player.flameStockPile);
        }
        else { flameFuelIcon.SetActive(false); }
    }

    // Use medkit and restore health
    public void UseMedkit()
    {
        // Call the player's UseMedkit method
        player.UseMedkit();

        // Update displayed medkit counts on the inventory screen
        medkitText.text = "Medkits: " +
            player.currentMedkits + "/" + player.maxMedkits;
        medkitScreenText.text = "Medkits: " +
            player.currentMedkits + "/" + player.maxMedkits;
    }
}

