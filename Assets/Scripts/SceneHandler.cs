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
}
