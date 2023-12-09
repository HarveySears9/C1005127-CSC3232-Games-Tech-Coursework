using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayMessage : MonoBehaviour
{
    // Reference to the text that displays the message
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Method to start the coroutine
    public void StartMessageDisplay(string message)
    {
        // Start the coroutine to show the message
        StartCoroutine(ShowMessage(message));
    }

    // Coroutine to display the message and clear it after a delay
    IEnumerator ShowMessage(string message)
    {
        // Set the text to message
        text.text = message;

        // Wait for 1 second
        yield return new WaitForSeconds(1f);

        // Clear the text
        text.text = "";
    }
}
