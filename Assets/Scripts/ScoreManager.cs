using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class ScoreManager : MonoBehaviour
{
    public int Score { get; private set; }
    public int totalLinesCleared { get; private set; }
    public int Level { get; private set; }

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI totallinesText;
    public TMP_InputField nameInputField;
    public Button saveButton;

    private Piece piece;

    private void Start()
    {
        Level = 1;
        piece = FindObjectOfType<Piece>();
        UpdateScoreText();
        saveButton.onClick.AddListener(OnSaveButtonClicked);
    }

    public void UpdateScore(int moveLinesCleared)
    {
        int points = CalculatePoints(moveLinesCleared, Level);
        Score += points;
        totalLinesCleared += moveLinesCleared;

        int linesForNextLevel = Level * 10;
        if (totalLinesCleared >= linesForNextLevel)
        {
            Level++;
            piece.UpdateStepDelay(Level);
        }

        UpdateScoreText();

        Debug.Log("Líneas eliminadas en este movimiento: " + moveLinesCleared);
        Debug.Log("Puntos obtenidos en este movimiento: " + points);
        Debug.Log("Puntuación total: " + Score);
        Debug.Log("Total de líneas eliminadas: " + totalLinesCleared);
        Debug.Log("Nivel actual: " + Level);
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Score.ToString();
            levelText.text = "Level: " + Level.ToString();
            totallinesText.text = "Lineas Eliminadas: " + totalLinesCleared.ToString();
        }
    }
    private int CalculatePoints(int moveLinesCleared, int level)
    {
        int basePoints = moveLinesCleared switch
        {
            1 => 40,
            2 => 100,
            3 => 300,
            4 => 1200,
            _ => 0,
        };

        return basePoints * (level + 1);
    }

    

    private void OnSaveButtonClicked()
    {
        string playerName = nameInputField.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            SaveScore(playerName, Score, totalLinesCleared, Level);
            Debug.Log("Score saved: " + playerName + " " + Score);
        }
        else
        {
            Debug.Log("Name is empty, score not saved.");
        }
    }

    private void SaveScore(string playerName, int score, int linesCleared, int level)
    {
        string path = Application.persistentDataPath + "/highscores.txt";
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine($"{playerName} {score} {linesCleared} {level}");
        }
    }
}
