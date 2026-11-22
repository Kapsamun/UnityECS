using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI элементы")]
    [Tooltip("Панель Game Over, которая включается при гибели игрока")]
    public GameObject GameOverPanel;

    private void Awake()
    {
        // Обязательно сбрасываем время на нормальную скорость при старте/перезапуске сцены
        Time.timeScale = 1f;

        // Реализация синглтона для быстрого доступа из ECS
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (GameOverPanel != null)
            GameOverPanel.SetActive(false);
    }

    /// <summary>
    /// Метод, вызываемый из ECS-системы при гибели игрока
    /// </summary>
    public void ShowGameOver()
    {
        Debug.Log("<color=red>[GameManager]</color> Игрок погиб! Включаем Game Over и ставим врагов на паузу.");

        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);
        }

        // Замораживаем игровое время (враги останавливаются, DeltaTime в ECS становится 0)
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Метод для кнопки рестарта
    /// </summary>
    public void RestartGame()
    {
        // Перед загрузкой сцены обязательно возвращаем нормальную скорость времени!
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}