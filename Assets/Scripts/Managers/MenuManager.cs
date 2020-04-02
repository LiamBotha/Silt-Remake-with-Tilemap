using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public void LoadGame()
    {
        SceneManager.LoadScene("LV1");
    }

    public void LoadEditor()
    {
        SceneManager.LoadScene("LevelLoader");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
