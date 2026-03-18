using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AspectRatio : MonoBehaviour
{
    void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Title")
        {
            Screen.SetResolution(1280, 1024, FullScreenMode.Windowed); // 5:4 resolution
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Game")
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed); // 16:9 resolution
        }
    }

}
