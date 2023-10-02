using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI leaderboardUI;
    public static LeaderboardManager Instance { get; private set; }
    private List<int> _highScores = new List<int>();
    private const int MaxScores = 5;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optionally, prevent the object from being destroyed on scene changes
        }
        else
        {
            Destroy(gameObject); // If an instance already exists, destroy this one
        }
        
        // Load scores from PlayerPrefs or initialize if not found
        LoadScores();
    }

    public void GameOver(int playerScore)
    {
        // Check if the player's score is a high score
        if (IsHighScore(playerScore))
        {
            // Insert the new high score and remove any excess scores
            _highScores.Add(playerScore);
            _highScores.Sort((a, b) => b.CompareTo(a)); // Sort in descending order
            if (_highScores.Count > MaxScores)
            {
                _highScores.RemoveAt(_highScores.Count - 1);
            }

            // Save the updated scores
            SaveScores();
        }

        // Show the leaderboard UI
        UpdateScores();
    }

    private bool IsHighScore(int score)
    {
        return _highScores.Count < MaxScores || score > _highScores[_highScores.Count - 1];
    }

    private void LoadScores()
    {
        _highScores.Clear();
        for (int i = 0; i < MaxScores; i++)
        {
            int score = PlayerPrefs.GetInt("HighScore" + i, 0);
            _highScores.Add(score);
        }
    }

    private void SaveScores()
    {
        for (int i = 0; i < _highScores.Count; i++)
        {
            PlayerPrefs.SetInt("HighScore" + i, _highScores[i]);
        }
        PlayerPrefs.Save();
    }

    private void UpdateScores()
    {
        // Update the leaderboard UI text
        string lbText = "Leaderboard:\n";
        for (int i = 0; i < _highScores.Count; i++)
        {
            lbText += "#" + (i + 1) + ": " + _highScores[i] + " pts\n \n";
        }
        leaderboardUI.text = lbText;
    }
    
}