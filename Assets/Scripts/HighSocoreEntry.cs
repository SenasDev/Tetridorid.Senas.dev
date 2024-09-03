// HighScoreEntry.cs
using System;

[Serializable]
public class HighScoreEntry
{
    public string PlayerName { get; set; }
    public int Score { get; set; }
    public int LinesCleared { get; set; }
    public int Level { get; set; }

    public HighScoreEntry(string playerName, int score, int linesCleared, int level)
    {
        PlayerName = playerName;
        Score = score;
        LinesCleared = linesCleared;
        Level = level;
    }
}
