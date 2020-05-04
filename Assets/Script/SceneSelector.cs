using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    public void LoadSmall()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadLarge()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadMaze()
    {
        SceneManager.LoadScene(2);
    }
}
