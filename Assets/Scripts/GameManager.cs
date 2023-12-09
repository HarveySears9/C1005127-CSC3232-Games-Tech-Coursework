using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Reference to the particle system for bullet hits
    public ParticleSystem BulletHit;

    // Arrays of zombies for different game states
    public GameObject[] zombieListOneKeycard;
    public GameObject[] zombieListEnd;

    // Reference to the ItemSpawner script
    [SerializeField] private ItemSpawner itemSpawner;

    // Game states
    public enum GameState { Start, OneKeycard, End };
    GameState state = GameState.Start;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize zombie kills in player preferences
        PlayerPrefs.SetInt("zombieKills", 0);
    }

    // Plays the blood effect when an enemy is hit with a bullet
    public void BulletHitEnemy(HandgunBullet bullet)
    {
        this.BulletHit.transform.position = bullet.transform.position;
        this.BulletHit.Play();
    }

    // Method called when the player wins the game
    public void Win()
    {
        Debug.Log("Player Has Won");
        PlayerPrefs.SetInt("won", 1);
        // Load the EndScreen scene
        SceneManager.LoadScene("EndScreen");
    }

    // Method called when the player loses the game
    public void Lose()
    {
        Debug.Log("Player Has Lost");
        PlayerPrefs.SetInt("won", 0);
        SceneManager.LoadScene("EndScreen");
    }

    // Method to change the game state to "OneKeycard"
    // This method is called when the player picks up their first keycard
    public void GameStateChangeOneKeycard()
    {
        state = GameState.OneKeycard;

        // Awaken zombies in the zombieListOneKeycard array
        foreach (var item in zombieListOneKeycard)
        {
            item.GetComponent<ZombieController>().AwakenZombie();
        }

        // Adjust the probability of spawning items in the ItemSpawner
        // This is to increase the difficulty as the player advances through the game
        itemSpawner.probOfAllPossibleItems = 0.6f;
    }

    // Method to change the game state to "End"
    // This method is called when the player completes the hacking minigame
    public void GameStateChangeEnd()
    {
        state = GameState.End;

        // Awaken zombies in the zombieListEnd array
        foreach (var item in zombieListEnd)
        {
            item.GetComponent<ZombieController>().AwakenZombie();
        }

        // Adjust the probability of spawning items in the ItemSpawner
        // This is to increase the difficulty as the player advances through the game
        itemSpawner.probOfAllPossibleItems = 0.4f;
    }
}
