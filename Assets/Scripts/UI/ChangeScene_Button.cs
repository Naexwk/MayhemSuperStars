using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Controlador de salida del juego
public class ChangeScene_Button : MonoBehaviour
{
    // Si se recibe la escena "Salir", salir del juego
    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
        if (scene == "Salir")
        {
            Application.Quit();
        }
    }
}