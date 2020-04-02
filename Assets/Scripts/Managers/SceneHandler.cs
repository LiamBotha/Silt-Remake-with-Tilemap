using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void LoadEditor()
    {
        SceneManager.LoadScene("LevelEditorScene");
    }

    public void ExitToMenu()
    {
        if (SceneManager.GetActiveScene().name == "LevelEditorScene")
            SceneManager.LoadScene("LevelLoader");
        else SceneManager.LoadScene("MenuScene");
    }

    public void RestartCurrentLevel()
    {
        var currentScene = SceneManager.GetActiveScene().name;

        if (currentScene.Contains("LevelLoader"))
        {
            Scene playerScene = SceneManager.GetSceneByName("PlayerScene");
            Scene UIScene = SceneManager.GetSceneByName("UIScene");

            SceneManager.UnloadSceneAsync(playerScene.name);
            SceneManager.UnloadSceneAsync(UIScene.name);

            GameObject.FindObjectOfType<LevelConverter>().LoadLevelAndPlay(true);
        }
        else SceneManager.LoadScene(currentScene);
    }

    public void ExitLevel()
    {
        if (SceneManager.GetSceneAt(0).name == "LevelLoader")
        {
            SceneManager.LoadScene("LevelLoader");
        }
        else
        {
            SceneManager.LoadScene("MenuScene");
        }
    }
}
