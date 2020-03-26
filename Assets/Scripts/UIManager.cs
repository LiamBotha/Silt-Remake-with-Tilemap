using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private int width, height;
    private Rect rect;
    private GUIStyle labelTime;
    private GUIStyle labelDeaths;
    private string currentTime;

    [SerializeField] GameObject completedPanel = null;
    [SerializeField] GameObject pausePanel = null;

    void Awake()
    {
        width = Screen.width;
        height = Screen.height;
        rect = new Rect(10, 10, width - 20, height - 20);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        pausePanel.SetActive(!pausePanel.activeSelf);

        Time.timeScale = pausePanel.activeSelf ? 0 : 1;
    }
    public void EndGame()
    {
        completedPanel.SetActive(true);

        Time.timeScale = 0;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        PlayerHandler.Reset();

        var currentScene = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(currentScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void OnGUI()
    {
        // Display the label at the center of the window.
        labelTime = new GUIStyle(GUI.skin.GetStyle("label"));
        labelDeaths = new GUIStyle(GUI.skin.GetStyle("label"));
        labelTime.alignment = TextAnchor.UpperRight;
        labelDeaths.alignment = TextAnchor.UpperLeft;

        // Modify the size of the font based on the window.
        labelTime.fontSize = 5 * (width / 200);
        labelDeaths.fontSize = 5 * (width / 200);

        // Obtain the current time.
        currentTime = Time.timeSinceLevelLoad.ToString("0:00");
        currentTime = "Time: " + currentTime;

        // Display the current time.
        GUI.Label(rect, currentTime, labelTime);
        GUI.Label(rect, "Deaths: " + PlayerHandler.deathCounter, labelDeaths);
    }
}
