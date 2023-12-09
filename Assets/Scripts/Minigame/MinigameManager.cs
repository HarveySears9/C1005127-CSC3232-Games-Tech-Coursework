using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public ComputerInteraction computer;

    public GameObject ball;

    public GameObject maze;

    private Vector3 ballResetPos;

    // Called when the script instance is being loaded
    void Awake()
    {
        // Find and assign the ComputerInteraction script in the scene
        computer = FindObjectOfType<ComputerInteraction>();

        // Store the initial position of the ball for resetting
        ballResetPos = ball.transform.position;
    }

    private void OnEnable()
    {
        // Reset the ball's position, scale, and the maze's rotation when the minigame is enabled
        ball.transform.position = ballResetPos;
        ball.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        maze.transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the Escape key is pressed to end the game with a loss
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndGame(false);
        }
    }

    // Method to end the minigame and inform the ComputerInteraction script
    public void EndGame(bool won)
    {
        computer.EndGame(won);
    }
}

