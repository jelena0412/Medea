using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLogic : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void StartLevel()
    {
        SceneManager.LoadScene(2);
    }    

    public void ExitGame()
    {
        Application.Quit();
    }

}
