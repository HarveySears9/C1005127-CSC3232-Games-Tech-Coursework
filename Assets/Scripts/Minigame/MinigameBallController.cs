using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameBallController : MonoBehaviour
{
    private new Rigidbody2D rigidbody;

    private bool canChange = true;

    public MinigameManager manager;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        manager = FindObjectOfType<MinigameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collider has the tag "MinigameChangeSize" and canChange is true
        if (collision.CompareTag("MinigameChangeSize") && canChange)
        {
            // Toggle gravity and change scale based on the current gravity scale
            if (rigidbody.gravityScale == 1f)
            {
                rigidbody.gravityScale = 0.25f;
                transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            }
            else
            {
                rigidbody.gravityScale = 1f;
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }

            // Set canChange to false to prevent immediate size changes
            canChange = false;

            // Start coroutine to delay the next size change
            StartCoroutine(DelayChange());
        }

        // Check if the collider has the tag "MinigameGoal"
        if (collision.CompareTag("MinigameGoal"))
        {
            // End the minigame with a win
            manager.EndGame(true);
        }
    }

    // Coroutine to delay the next size change
    IEnumerator DelayChange()
    {
        yield return new WaitForSeconds(1.5f);

        canChange = true;
    }
}

