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

    public float startTime = 0.0f;
    private string currentTime;

    [SerializeField] GameObject completedPanel = null;
    [SerializeField] GameObject pausePanel = null;
    [SerializeField] TMPro.TextMeshProUGUI completedTimeText = null;

    SceneHandler sceneHandler;

    void Awake()
    {
        sceneHandler = GameObject.FindObjectOfType<SceneHandler>();

        if (sceneHandler == null)
            sceneHandler = new GameObject("SceneHandler").AddComponent<SceneHandler>();

        width = Screen.width;
        height = Screen.height;
        rect = new Rect(10, 10, width - 20, height - 20);
        startTime = Time.time;

        Time.timeScale = 1;
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

        completedTimeText.text = "Level Completed:" + Environment.NewLine + Time.timeSinceLevelLoad.ToString("0:00");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        PlayerHandler.Reset();

        sceneHandler.RestartCurrentLevel();
    }

    public void ExitLevel()
    {
        sceneHandler.ExitLevel();
    }

    void OnGUI()
    {
        // Display the label at the center of the window.
        labelTime = new GUIStyle(GUI.skin.GetStyle("label"));
        labelDeaths = new GUIStyle(GUI.skin.GetStyle("label"));
        labelTime.alignment = TextAnchor.UpperLeft;
        labelDeaths.alignment = TextAnchor.UpperLeft;

        labelDeaths.contentOffset = new Vector2(0, 0.05f * height);

        // Modify the size of the font based on the window.
        labelTime.fontSize = 5 * (width / 200);
        labelDeaths.fontSize = 5 * (width / 200);

        // Obtain the current time.
        currentTime = (Time.time - startTime).ToString("0:00");
        currentTime = "Time: " + currentTime;

        // Display the current time.
        GUI.Label(rect, currentTime, labelTime);
        GUI.Label(rect, "Deaths: " + PlayerHandler.deathCounter, labelDeaths);
    }
}
