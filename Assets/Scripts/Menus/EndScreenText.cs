using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScreenText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI EndText;   
    [SerializeField] private TextMeshProUGUI SummaryText;

    // Start is called before the first frame update
    void Start()
    {
        // retrieve PlayerPrefs (stores values across scenes)
        int won = PlayerPrefs.GetInt("won");
        int kills = PlayerPrefs.GetInt("zombieKills");

        if (won == 1)
        {
            // Calculate score and decide rank
            int score = CalculateScore(true, kills);
            string rank = DecideRank(score);

            // Update text with score and rank
            EndText.text = "You Survived!";
            SummaryText.text = "Zombies killed: " + kills + " x 200\r\nEscape Bonus: 1000\r\nTotal Score: " + score + "\r\n\r\nRank: " + rank + "\r\n";
        }
        else
        {
            // Calculate score and decide rank
            int score = CalculateScore(false, kills);
            string rank = DecideRank(score);

            // Update text with score and rank
            EndText.text = "You Died...";
            SummaryText.text = "Zombies killed: " + kills + " x 200\r\nTotal Score: " + score + "\r\n\r\nRank: " + rank + "\r\n";
        }
    }

    int CalculateScore(bool won, int kills)
    {
        if (won)
        {
            // If the player won, calculate the score with a bonus for winning
            return (kills * 200) + 1000;
        }
        else
        {
            // If the player lost, calculate the score without a winning bonus
            return (kills * 200);
        }
    }

    // Gives the player a rank based on their score
    string DecideRank(int score)
    {
        if (score > 2000) { return "S"; }
        else if (score > 1200) { return "A"; }
        else if (score > 800) { return "B"; }
        else if (score > 500) { return "C"; }
        else { return "D"; }
    }
}

