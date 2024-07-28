using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuInteractions : MonoBehaviour
{
    public void quit()
    {
        Application.Quit();
    }

    public void openTraining()
    {
        SceneManager.LoadSceneAsync("Scenes/Training");
    }
    public void openRace()
    {
        SceneManager.LoadSceneAsync("Scenes/Racing");
    }

}
