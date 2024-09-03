// HighScoreManager.cs
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    private string filePath;
    public List<HighScoreEntry> highScoreEntries;

    private void Awake()
    {
        filePath = Application.persistentDataPath + "/highscores.txt";
        highScoreEntries = new List<HighScoreEntry>();

        LoadHighScores();
        SortHighScores();
    }

    private void LoadHighScores()
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    string[] parts = line.Split(' ');
                    if (parts.Length == 4)
                    {
                        string playerName = parts[0];
                        int score = int.Parse(parts[1]);
                        int linesCleared = int.Parse(parts[2]);
                        int level = int.Parse(parts[3]);

                        HighScoreEntry entry = new HighScoreEntry(playerName, score, linesCleared, level);
                        highScoreEntries.Add(entry);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Highscores file not found.");
        }
    }

    private void SortHighScores()
    {
        highScoreEntries.Sort((x, y) => y.Score.CompareTo(x.Score));
    }

    public List<HighScoreEntry> GetTopHighScores(int count)
    {
        return highScoreEntries.GetRange(0, Mathf.Min(count, highScoreEntries.Count));
    }
}
