using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";
    public string introSceneName = "IntroScene"; // เพิ่มตัวแปรฉาก Intro
    public string gameSceneName = "MainGame";

    // ฟังก์ชันสำหรับปุ่ม Play ในหน้า MainMenu
    public void PlayGame()
    {
        Time.timeScale = 1f; // บังคับเวลาให้เดิน
        SceneManager.LoadScene(introSceneName); // โหลดไป Intro ก่อน
    }

    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        if (Conductor.Instance != null && Conductor.Instance.musicSource != null)
        {
            Conductor.Instance.musicSource.Stop();
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}