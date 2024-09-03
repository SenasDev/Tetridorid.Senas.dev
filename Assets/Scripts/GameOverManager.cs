using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject puntajePanel;
    public GameObject puntaje;
    public TextMeshProUGUI finalScore; 
    public ScoreManager scoreManager; 

    void Start()
    {
        gameOverPanel.SetActive(false);
        puntajePanel.SetActive(true);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        puntajePanel.SetActive(false);
        puntaje.SetActive(false);
        finalScore.text = "Final Score: " + scoreManager.Score.ToString();
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        puntajePanel.SetActive(false);
        puntaje.SetActive(true);
        
        Time.timeScale = 1f; 
        scoreManager.UpdateScore(0); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}


