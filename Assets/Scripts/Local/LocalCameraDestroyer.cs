using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class LocalCameraDestroyer : MonoBehaviour
{
    // Escuchar cambios de escena
    private void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Ejecutar funciones de escena de juego
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if ( this != null) {
            if (scene.name == "MainMenuScene") {
                Destroy(this.gameObject);
            }

            if (scene.name == "SampleScene") {
                GameObject mainCamera = GameObject.FindWithTag("MainCamera");
                if (mainCamera != null) {
                    Destroy(mainCamera.gameObject);
                }
                
            }
        }
        
    }
}
